using GoMore_C2B1.DataContext;
using GoMore_C2B1.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GoMore_C2B1.Controllers
{
    public class FactoryController : Controller
    {
        private ApplicationDbContext FC = new ApplicationDbContext();
        // GET: Factory
        public ActionResult Upload()
        {
            if (Session["Account"] != null)
            {
                string Uploader = Session["Account"].ToString();
                List<FactoryModel> FCobj = FC.FCM.Where(x => x.Uploader.Equals(Uploader)).ToList();
                if (FCobj != null)
                {
                    return View(FCobj);
                }
                TempData["Msg"] = "Get uploaded history fail.";
                return View();

            }
            TempData["Msg"] = "Login first!";
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Parameter()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Client(HttpPostedFileBase file,string option)
        {
            FactoryModel factoryModel = new FactoryModel();
            try
            {
                var jsonValues = new Dictionary<string, string>
                {
                    { "option", option }
                };
                StringContent sc = new StringContent(JsonConvert.SerializeObject(jsonValues), UnicodeEncoding.UTF8);

                HttpClient http = new HttpClient();
                string url = "http://140.118.121.104:17402/demoapi/GASLabReceiveFile";
                MultipartFormDataContent mulContent = new MultipartFormDataContent("----WebKitFormBoundaryrXRBKlhEeCbfHIY");

                var fileContent = new StreamContent(file.InputStream);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                mulContent.Add(fileContent, "file", file.FileName);
                mulContent.Add(sc, "input1");
                await http.PostAsync(url, mulContent);

                if (ModelState.IsValid)
                {
                    Random generator = new Random();
                    
                    factoryModel.ID = generator.Next(0, 1000000).ToString("D6");
                    factoryModel.FileName = file.FileName.Split('.')[0];
                    factoryModel.UploadTime = DateTime.UtcNow.ToString();
                    factoryModel.Uploader = Session["Account"].ToString();
                    factoryModel.FileType = file.FileName.Split('.')[1];
                    FC.FCM.Add(factoryModel);
                    FC.SaveChanges();
                }
                TempData["Msg"] = "Post Successful";
            }
            catch (Exception)
            {
                TempData["Msg"] = "Post fail";
            }

            return RedirectToAction("Upload", "Factory");
        }

        [HttpPost]
        public async Task RTLabReceiveFile(HttpPostedFileBase file)
        {

            Console.WriteLine("recevied your file,file name is :" + file.FileName);

            await SaveStream(file.InputStream, @"C:\FCFTP", file.FileName);


        }

        static readonly CancellationTokenSource s_cts = new CancellationTokenSource();
        async Task SaveStream(Stream fileStream, string destinationFolder, string destinationFileName)
        {


            if (!Directory.Exists(destinationFolder))
                Directory.CreateDirectory(destinationFolder);

            string path = Path.Combine(destinationFolder, destinationFileName);

            using (FileStream outputFileStream = new FileStream(path, FileMode.CreateNew))
            {
                try
                {
                    await fileStream.CopyToAsync(outputFileStream);
                }
                catch (OperationCanceledException)
                {

                }
                finally
                {
                    s_cts.Dispose();
                }

            }
        }

        public static byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        public static bool ServicePing(string address, int port)
        {
            bool serviceOnline;

            TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(address, port);
                serviceOnline = true;
            }
            catch (Exception)
            {
                serviceOnline = false;
            }

            return serviceOnline;
        }

    }
}
