using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Markup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using SOSResources.Data;
using SOSResources.Models;

namespace SOSResources.Pages.Utilities
{
    public class ImportModel : PageModel
    {
        private readonly SOSResources.Data.SOSContext _context;

        public ImportModel(SOSResources.Data.SOSContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public Textbook[] Textbooks { get; set; }
        
        public string CSVString { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string inputCSVString)
        {
            if (String.IsNullOrEmpty(inputCSVString)){
                return Page();
            } else{
                CSVString = inputCSVString;
            }

            string[] lines = CSVString.Split("\n");
            if (lines[0].Contains("Title,Author")){
                lines = lines[1..^0];
            }
            Console.WriteLine(lines[0]);

            Dictionary<(string, string, string, bool), int> textbookCopies = new Dictionary<(string, string, string, bool), int>();

            //parse lines and convert duplicate textbooks to copies
            foreach (string l in lines){
                var vals = l.Split(",");

                string title = vals[0];
                string author = vals[1];
                string edition = vals[2];
                bool checkedOut = vals[3] == "YES" ? true : false;

                var tbVals = (title, author, edition, checkedOut);

                if (textbookCopies.ContainsKey(tbVals)){
                    textbookCopies[tbVals] = textbookCopies[tbVals] + 1;
                } else {
                    textbookCopies.Add(tbVals, 1);
                }
            }

            // create textbooks and copies from dictionary
            foreach((string, string, string, bool) k in textbookCopies.Keys){
                Textbook tb = null;
                (string title, string author, string edition, bool checkedOut) = k;
                
                var matches = _context.Textbooks.Where(t => t.Title.ToUpper().Equals(title.ToUpper()) && t.Author.ToUpper().Equals(author.ToUpper()));
                if (!matches.IsNullOrEmpty()){
                    Console.WriteLine(matches.Count());
                    if (String.IsNullOrWhiteSpace(edition)){
                        var editionMatch = matches.Where(t => String.IsNullOrWhiteSpace(t.Edition));
                        if (!editionMatch.IsNullOrEmpty()){
                            tb = editionMatch.ElementAt(0);
                        }
                    } else {
                        var editionMatch = matches.Where(t => t.Edition.Equals(edition));
                        if (!editionMatch.IsNullOrEmpty()){
                            tb = editionMatch.ElementAt(0);
                        }
                    }
                    
                }
                if (tb == null){
                    tb = new Textbook{
                        Title = title,
                        Author = author
                    };
                    if (!String.IsNullOrWhiteSpace(edition)){
                        tb.Edition = edition;
                    }
                    _context.Textbooks.Add(tb);
                }

                for (int i = 0; i < textbookCopies[k]; i++) {
                    Copy c = new Copy{
                        textbook = tb,
                        CheckedOut = checkedOut
                    };
                    _context.Copies.Add(c);
                };

            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Textbooks/Index");
        }
    }
}
