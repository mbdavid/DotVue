using DotVue;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace WebApp.Pages
{
    public class Upload : ViewModel
    {
        public void Upload1(IFormFile file1)
        {
            this.ClientScript.Alert(file1?.Length.ToString() ?? "no-file");
        }

        public void Upload2(IFormFile fileA, IFormFile fileB)
        {
            this.ClientScript.Alert(fileA.Length+ " - " + fileB.Length);
        }

        public void UploadMultiple(IList<IFormFile> files)
        {
            this.ClientScript.Alert("Total: " + files.Count);
        }

        public void UploadAndParams(int a, IList<IFormFile> files, string b)
        {
            this.ClientScript.Alert("Total: " + files.Count + "(a=" + a + "-b=" + b + ")");
        }
    }
}