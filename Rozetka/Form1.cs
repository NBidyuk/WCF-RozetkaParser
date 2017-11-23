/*Реализовать приложение, которое выводит информацию о ноутбуках с сайта Rozetka.com.ua.
Должна быть выведена минимум такая информация по каждому ноутбуку: название, цена, тип процессора, размер ОЗУ.
В приложении должен использоваться асинхронные методы async await.*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Xml;
using System.Net.Http;
using System.Reflection;

namespace Rozetka
{
    public partial class Form1 : Form
    {
        private List<NoteBook> productList;
        public Form1()
        {
            InitializeComponent();
            productList = new List<NoteBook>();
            Type type = listView1.GetType();
            PropertyInfo propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            propertyInfo.SetValue(listView1, true, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
             {

                 
                 WebClient client = new WebClient();
                 client.Encoding = UTF8Encoding.UTF8;
                 HtmlAgilityPack.HtmlDocument resultDoc = new HtmlAgilityPack.HtmlDocument();

                 int pageCount = 1;
                


                 for (;pageCount<=35;pageCount++)
                 {
                     string path = String.Format("http://rozetka.com.ua/notebooks/c80004/filter/page={0}/", pageCount);
                     string urlData = client.DownloadString(path);




                     resultDoc.LoadHtml(urlData);
                     var allDivlist = resultDoc.DocumentNode.Descendants().Where
                 (x => (x.Name == "div" && x.Attributes["name"] != null && x.Attributes["name"].Value.Equals("goods_list"))).ToList();



                     var AllProductsNodes = allDivlist[0].Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("g-i-tile-i-box"))).ToList();

                     foreach (var obj in AllProductsNodes)
                     {

                         var listName = obj.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("g-i-tile-i-title"))).ToList()[0].Descendants().Where(x => (x.Name == "a")).ToList();
                         var name = listName[0].InnerText;
                         var link = listName[0].GetAttributeValue("href", null);

                         var listDesc = obj.Descendants().Where(x => (x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("g-i-tile-short-detail"))).ToList();
                         var description = listDesc[0].Descendants().Where(x => (x.Name == "li")).ToList()[0].InnerText;

                         

                         var listPrice = obj.Descendants().Where(x => (x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("g-price-uah"))).ToList();
                         var price = "";
                         if (listPrice.Count>0)
                         {
                             var lprice = listPrice[0].Descendants().Where(x => (x.Name == "div")).ToList();
                             price = listPrice[0].InnerText;
                         }
                         else
                         {
                             price = "no info";
                         }
                         



                         NoteBook newLaptop = new NoteBook()
                         {
                             Name = name,
                             Description = description,
                             Price = price + " UAH",
                             Link = link

                         };

                         productList.Add(newLaptop);

                         ListViewItem item = new ListViewItem(newLaptop.Name);
                         item.SubItems.Add(newLaptop.Description);
                         item.SubItems.Add(newLaptop.Price);

                         item.SubItems.Add(newLaptop.Link);
                         listView1.HotTracking = true;

                         listView1.Items.Add(item);

                     }


               
                 }
             });
        }




        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            var hit = listView1.HitTest(e.Location);
            if (hit.SubItem != null && hit.SubItem == hit.Item.SubItems[3])
                listView1.Cursor = Cursors.Hand;
            else listView1.Cursor = Cursors.Default;
        }


        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            var hit = listView1.HitTest(e.Location);
            if (hit.SubItem != null && hit.SubItem == hit.Item.SubItems[3])
            {
                var url = new Uri(hit.SubItem.Text);
                System.Diagnostics.Process.Start(url.ToString());
            }
        }
    }



    public class NoteBook

    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Link { get; set; }

    }

}

