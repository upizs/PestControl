using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PestControl.Data.Contracts;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Projects
{
    public class ListModel : PageModel
    {
        private readonly IProjectRepository _projectRepository;

        public ListModel(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public IEnumerable<Project> Projects { get; set; }
        [TempData]
        public string Message { get; set; }
        public async Task OnGet()
        {
            Projects = await _projectRepository.GetAllAsync();
        }
    }
}
