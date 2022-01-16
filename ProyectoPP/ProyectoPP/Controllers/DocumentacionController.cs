using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ProyectoPP.Controllers
{
    public class DocumentacionController : Controller
    {
        private ProyectoPP.Models.patopurificEntitiesGeneral db = new ProyectoPP.Models.patopurificEntitiesGeneral();

        // GET: Documentacion
        public ActionResult Index()
        {
            return View(GetFiles());
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            var docId = db.Documentacion.Count();

            docId++;
            
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "INSERT INTO Documentacion VALUES (@id, @nombre, @ContentType, @Data)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", docId);
                    cmd.Parameters.AddWithValue("@nombre", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@ContentType", postedFile.ContentType);
                    cmd.Parameters.AddWithValue("@Data", bytes);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return View(GetFiles());
        }

        [HttpPost]
        public ActionResult DownloadFile(int? fileId)
        {
            byte[] bytes;
            string fileName, contentType;
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT nombre, Data, ContentType FROM Documentacion WHERE id=@Id";
                    cmd.Parameters.AddWithValue("@Id", fileId);
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        sdr.Read();
                        bytes = (byte[])sdr["Data"];
                        contentType = sdr["ContentType"].ToString();
                        fileName = sdr["nombre"].ToString();
                    }
                    con.Close();
                }
            }

            return File(bytes, contentType, fileName);
        }

        private static List<ProyectoPP.Models.DocumentacionModel> GetFiles()
        {
            List<ProyectoPP.Models.DocumentacionModel> files = new List<ProyectoPP.Models.DocumentacionModel>();
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT id, nombre FROM Documentacion"))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            files.Add(new Models.DocumentacionModel
                            {
                                Id = Convert.ToInt32(sdr["id"]),
                                Nombre = sdr["nombre"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return files;
        }
    }
}