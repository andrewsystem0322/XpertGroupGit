using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XpertGroup.Models;

namespace XpertGroup.Controllers
{
    public class HomeController : Controller
    {
        int contadorFilas = -1;

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult CalcularPuntaje()
        {
            string s = "";
            string mensaje = "";
            bool resultado;
            int calculo = 0;
            bool continuarCalculo = true;
            

            List<string> palabrasPositvas = new List<string>
            {
                "Gracias",
                "Buena Atención",
                "Muchas Gracias"
            };

            try
            {
                //abriendo el archivo
                using (StreamReader sr = System.IO.File.OpenText(@"C:\Users\Andres Arias\source\repos\XpertGroup\XpertGroup\wwwroot\FilesChats\historial_de_conversaciones.txt"))
                {
                    bool contarSiguiente = true;
                    //recorro las filas del archivo
                    while ((s = sr.ReadLine()) != null)
                    {
                        //validamos nueva conversacion
                        if (s.Trim() =="")
                        {
                            if (contarSiguiente)//calculamos total filas por conversacion
                            {
                                if (contadorFilas == 1)
                                    calculo -= 100;
                                else if (contadorFilas > 1 && contadorFilas <= 5)
                                    calculo += 20;
                                else if (contadorFilas > 5)
                                    calculo += 10;
                            }

                            contadorFilas = -1;
                            continuarCalculo = true;
                        }
                        else
                            contarSiguiente = true;

                        //validamos si se debe calcular
                        if (s.Trim() != "")
                        {
                            contadorFilas += 1;

                            //validamos excelente servicio
                            if (s.ToUpper().Contains("EXCELENTE SERVICIO"))
                            {
                                calculo += 100;
                                continuarCalculo = false;
                            }

                            //validamos si podemos continuar calculando despues de excelente servicio
                            if (continuarCalculo)
                            {
                                //calcular palabras positivas
                                if (palabrasPositvas.Any(x => x.Contains(s)))
                                {
                                    calculo += 10;
                                }

                                //calcular urgente

                                if (Enumerable.Range(1, 2).Contains(Regex.Matches(s.ToUpper(), "URGENTE").Count))
                                    calculo -= 5;
                                else if (Regex.Matches(s.ToUpper(), "URGENTE").Count > 2)
                                    calculo -= 10;
                            }
                        }
                            
                    }
                }

                //volvemos a calcular la cantidad de lineas debido a que en la ultima linea se sale del ciclo y no lo cuenta
                //calculamos total filas por conversacion
                if (contadorFilas == 1)
                    calculo -= 100;
                else if (contadorFilas > 1 && contadorFilas <= 5)
                    calculo += 20;
                else if (contadorFilas > 5)
                    calculo += 10;
                // fin calculo de la ultima linea para el contador de lineas

                mensaje = "Calculado correctamente";
                resultado = true;
            }
            catch (Exception ex)
            {
                mensaje = "Se genero un error. El error es: " + ex.Message;
                resultado = false;
            }
           

            return Json(new { message = mensaje, success = resultado, calificacion = calculo });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
