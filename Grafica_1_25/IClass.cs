using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafica_1_25
{
    internal interface IClass
    {
        void setCenter(Vertex center);
        void addElement(string name, IClass element);
        void deleteElement(string name);
        object getElement(string name);
        void Draw();
    }
}
