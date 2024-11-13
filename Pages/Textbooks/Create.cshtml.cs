using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HendrixSOSResources.Data;
using SOSResources.Models;

namespace HendrixSOSResources.Pages.Textbooks
{
    public class CreateModel : PageModel
    {
        private readonly HendrixSOSResources.Data.SOSContext _context;

        public CreateModel(HendrixSOSResources.Data.SOSContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Textbook Textbook { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Textbooks.Add(Textbook);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}