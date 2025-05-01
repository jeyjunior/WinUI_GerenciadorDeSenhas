using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace GSApplication.Services
{
    public static class NavigationService
    {
        public static Frame MainFrame;

        public static void NavegarPara(Type pageType)
        {
            MainFrame?.Navigate(pageType);
        }
    }
}
