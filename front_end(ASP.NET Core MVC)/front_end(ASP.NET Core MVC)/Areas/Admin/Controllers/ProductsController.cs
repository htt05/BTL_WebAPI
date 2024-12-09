using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using front_end_ASP.NET_Core_MVC_.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using X.PagedList.Extensions;

namespace front_end_ASP.NET_Core_MVC_.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductsController : Controller
    {

        private readonly string uri = "https://localhost:44377/api/";
        HttpClient _client;
        public ProductsController()
        {
            _client = new HttpClient { BaseAddress = new Uri(uri) };
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index(string? searchName, int? categoryId, int page = 1)
        {
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var categories = JsonConvert.DeserializeObject<List<Category>>(await _client.GetStringAsync(uri + "Categories"));
            categories.Insert(0, new Category { CategoryId = 0, CategoryName = "----------Chose Category----------" });
            ViewBag.categoryId = new SelectList(categories, "CategoryId", "CategoryName", categoryId);
            ViewBag.id = categoryId;

            var response = await _client.GetStringAsync(uri + $"Products?searchName={searchName}&categoryId={categoryId}");
            var products = JsonConvert.DeserializeObject<List<Product>>(response);
            //phan trang
            if (page < 1)
            {
                page = 1;
            }
            int pageSize = 5;
            var results = products.ToPagedList(page, pageSize);
            ViewBag.searchName = searchName;
            return View(results);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var product = JsonConvert.DeserializeObject<Product>(await _client.GetStringAsync(uri + "Products/" + id));
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            var Data = new List<SelectListItem>
        {
            new SelectListItem { Value = "0", Text = "Sold out" },
            new SelectListItem { Value = "1", Text = "Available" },
        };
            ViewBag.Status = new SelectList(Data, "Value", "Text");
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var categories = JsonConvert.DeserializeObject<List<Category>>(await _client.GetStringAsync(uri + "Categories"));
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Price,Discount,ProductCount,Description,Status,CategoryId")]
        Product product, IFormFile picture, List<IFormFile> photos)
        {
            var Data = new List<SelectListItem>
        {
            new SelectListItem { Value = "0", Text = "Sold out" },
            new SelectListItem { Value = "1", Text = "Available" },
        };
            ViewBag.Status = new SelectList(Data, "Value", "Text", product);
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var categories = JsonConvert.DeserializeObject<List<Category>>(await _client.GetStringAsync(uri + "Categories"));
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
            if (ModelState.IsValid)
            {
                if (picture != null && picture.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/product", picture.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await picture.CopyToAsync(stream);
                    }
                    product.Picture = "/image/product/" + picture.FileName;
                }

                var response = await _client.PostAsJsonAsync("Products", product);
                if (response.IsSuccessStatusCode)
                {
                    if (photos != null && photos.Count > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/product/photo-detail/");
                        foreach (var file in photos)
                        {
                            if (file.Length > 0)
                            {
                                var path = Path.Combine(uploadsFolder, file.FileName);
                                using (var stream = System.IO.File.Create(path))
                                {
                                    await file.CopyToAsync(stream);
                                }
                                Image img = new Image()
                                {
                                    Picture = "/image/product/photo-detail/" + file.FileName,
                                    ProductId = product.ProductId
                                };
                                await _client.PostAsJsonAsync("ProductImgs", img);
                            }
                        }
                    }

                    var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                    TempData["success"] = responseMessage?.Message ?? "Product created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                    ViewBag.error = responseMessage?.Message ?? "An error occurred while creating the product.";
                }
            }

            return View(product);
        }


        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (id == null)
            {
                return NotFound();
            }

            var product = JsonConvert.DeserializeObject<Product>(await _client.GetStringAsync(uri + "Products/" + id));
            if (product == null)
            {
                return NotFound();
            }
            var Data = new List<SelectListItem>
                {
                    new SelectListItem { Value = "0", Text = "Sold out" },
                    new SelectListItem { Value = "1", Text = "Available" },
                };
            ViewBag.Status = new SelectList(Data, "Value", "Text");

            var categories = JsonConvert.DeserializeObject<List<Category>>(await _client.GetStringAsync(uri + "Categories"));
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName");
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProductId,ProductName,Price,Discount,ProductCount,Description,Status,CategoryId")] Product product,
    IFormFile? picture, List<IFormFile>? photos, string? old_picture)
        {
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var Data = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Sold out" },
                new SelectListItem { Value = "1", Text = "Available" },
            };
            ViewBag.Status = new SelectList(Data, "Value", "Text");

            var categories = JsonConvert.DeserializeObject<List<Category>>(await _client.GetStringAsync(uri + "Categories"));
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName");
            if (id != product.ProductId.ToString())
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (picture != null && picture.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/product", picture.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await picture.CopyToAsync(stream);
                    }
                    product.Picture = "/image/product/" + picture.FileName;
                }
                else
                {
                    product.Picture = old_picture;
                }

                var response = await _client.PutAsJsonAsync("Products/" + product.ProductId, product);
                var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                if (response.IsSuccessStatusCode)
                {
                    if (photos != null && photos.Count > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/product/photo-detail/");
                        foreach (var file in photos)
                        {
                            if (file.Length > 0)
                            {
                                var path = Path.Combine(uploadsFolder, file.FileName);
                                using (var stream = System.IO.File.Create(path))
                                {
                                    await file.CopyToAsync(stream);
                                }
                                Image img = new Image()
                                {
                                    Picture = "/image/product/photo-detail/" + file.FileName,
                                    ProductId = product.ProductId
                                };
                                await _client.PostAsJsonAsync("ProductImgs", img);
                            }
                        }
                    }


                    TempData["success"] = responseMessage?.Message ?? "Product updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.error = responseMessage?.Message ?? "An error occurred while updating the product.";
                    return View(product);
                }
            }
            return View(product);
        }


        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (id == null)
            {
                return NotFound();
            }

            var product = JsonConvert.DeserializeObject<Product>(await _client.GetStringAsync(uri + "Products/" + id));
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.DeleteAsync("Products/" + id);
            var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {

                TempData["success"] = responseMessage.Message;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = JsonConvert.DeserializeObject<Product>(await _client.GetStringAsync(uri + "Products/" + id));
                ViewBag.error = responseMessage.Message;
                return View(product);
            }
        }
    }
}
