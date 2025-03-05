using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SOS_Resources.Pages;

public class AdminsModel : PageModel
{
    private readonly ILogger<AdminsModel> _logger;

    public AdminsModel(ILogger<AdminsModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}

