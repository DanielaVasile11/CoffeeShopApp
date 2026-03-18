using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeShopAPI.Data;
using CoffeeShopAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic; // Necesar pentru Dictionary (TF-IDF)

namespace CoffeeShopAPI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // =================================================================
        // GET: /Products/Index (Meniu cu Algoritm Căutare TF-IDF)
        // =================================================================
        public async Task<IActionResult> Index(string SearchString, string category)
        {
            // Preluăm toate produsele în memorie pentru a putea aplica algoritmul matematic
            var products = await _context.Products.ToListAsync();

            // Filtrare clasică pe categorii
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category).ToList();
                ViewData["Title"] = "Meniu - " + category;
            }

            // ==========================================================
            // IMPLEMENTARE ALGORITM TF-IDF (PENTRU CERINȚA PROFESORULUI)
            // ==========================================================
            if (!string.IsNullOrEmpty(SearchString))
            {
                // Împărțim textul căutat în cuvinte separate (termens)
                var searchTerms = SearchString.ToLower().Split(new[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
                int totalDocuments = products.Count;

                // 1. Calculare IDF (Inverse Document Frequency) pentru fiecare cuvânt căutat
                var idfScores = new Dictionary<string, double>();
                foreach (var term in searchTerms)
                {
                    // Câte produse conțin acest cuvânt în titlu?
                    int docsWithTerm = products.Count(p => p.Name.ToLower().Contains(term));

                    // Formula IDF: Log10(Număr total documente / Documente care conțin termenul)
                    idfScores[term] = docsWithTerm > 0 ? Math.Log10((double)totalDocuments / docsWithTerm) : 0;
                }

                // 2. Calculare TF-IDF pentru fiecare produs (document)
                var scoredProducts = products.Select(p =>
                {
                    double totalScore = 0;
                    string productNameLower = p.Name.ToLower();
                    var nameWords = productNameLower.Split(new[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var term in searchTerms)
                    {
                        // TF (Term Frequency): de câte ori apare cuvântul în titlul produsului
                        int tf = nameWords.Count(w => w.Contains(term));

                        if (tf > 0 && idfScores.ContainsKey(term))
                        {
                            totalScore += tf * idfScores[term];
                        }
                    }

                    return new { Product = p, Score = totalScore };
                });

                // 3. Ordonare după relevanță și filtrare (eliminăm produsele cu scor 0)
                products = scoredProducts
                            .Where(x => x.Score > 0)
                            .OrderByDescending(x => x.Score)
                            .Select(x => x.Product)
                            .ToList();

                ViewData["Title"] = "Rezultate căutare";
            }

            if (string.IsNullOrEmpty(category) && string.IsNullOrEmpty(SearchString))
            {
                ViewData["Title"] = "Meniul Cafenelei (Toate Produsele)";
            }

            return View(products);
        }

        // =================================================================
        // ADMIN: CREATE
        // =================================================================
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Description,Category")] Product product, IFormFile pdfFile)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                if (pdfFile != null && pdfFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "pdfs");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(pdfFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await pdfFile.CopyToAsync(fileStream);
                    }
                    product.PdfDocumentPath = "/pdfs/" + uniqueFileName;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // =================================================================
        // ADMIN: EDIT 
        // =================================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,Category,PdfDocumentPath")] Product product, IFormFile pdfFile)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (pdfFile != null && pdfFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "pdfs");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(pdfFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await pdfFile.CopyToAsync(fileStream);
                        }
                        product.PdfDocumentPath = "/pdfs/" + uniqueFileName;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // =================================================================
        // ADMIN: DELETE
        // =================================================================
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // DETAILS
        // =================================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}