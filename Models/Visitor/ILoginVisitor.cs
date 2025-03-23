using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMusic.Models.Visitor
{
    public interface ILoginVisitor
    {
        //LoginController

        void Visit(Customer customer);
        void Visit(User admin);
    }
}
