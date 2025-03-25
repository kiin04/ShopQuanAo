using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebMusic.Models.Commannd
{
    internal interface ICommand
    {
        //DetailController AddComment
        JsonResult Execute();
    }
}
