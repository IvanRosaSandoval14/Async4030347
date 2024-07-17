using System.Diagnostics;

namespace Async4030347
{
    public partial class Form1 : Form
    {
        HttpClient httpClient = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes\resultado-secuencial");
            var destinoBaseParalelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecucion(destinoBaseParalelo, destinoBaseSecuencial);

            Console.WriteLine("inicio");
            List<Imagen> imagenes = ObtenerImagenes();




            var sw = new Stopwatch();
            sw.Start();

            foreach (var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial, imagen);
            }

            Console.WriteLine("Secuencial - duracion en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.0);

            sw.Restart();
            sw.Start();

            var TareasEnumerable = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBaseParalelo, imagen);
            });

            await Task.WhenAll(TareasEnumerable);

            Console.WriteLine("Paralelo - duracion en segundos {0}",
                sw.ElapsedMilliseconds / 1000.0);

            sw.Stop();

            pictureBox1.Visible = false;
        }

        private async Task ProcesarImagen(string directorio, Imagen imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();

            Bitmap bitmap;
            using (var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);
        }

        private static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();

            for (int i = 0; i < 7; i++)
            {
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Sancti {i}.jpeg",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f2/Castillo_de_Sancti_Petri.JPG/800px-Castillo_de_Sancti_Petri.JPG"
                    });
                imagenes.Add(
                new Imagen()
                {
                    Nombre = $"Santa María {i}.jpg",
                    URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b0/Catedral_de_Santa_Mar%C3%ADa_de_Burgos_-_01.jpg/400px-Catedral_de_Santa_Mar%C3%ADa_de_Burgos_-_01.jpg"
                });
                imagenes.Add(
                new Imagen()
                {
                    Nombre = $"Congreso {i}.jpg",
                    URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/51/Congreso_de_los_Diputados_%28Espa%C3%B1a%29_17.jpg/800px-Congreso_de_los_Diputados_%28Espa%C3%B1a%29_17.jpg"
                }
                );


            }
            return imagenes;
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecucion(string destinoBaseParalelo,
            string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoBaseParalelo))
            {
                Directory.CreateDirectory(destinoBaseParalelo);
            }
            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);
            }

            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoBaseParalelo);
        }

        private async Task<string> ProcesamientoLargo()
        {
            await Task.Delay(5000);
            return "Felipe";

        }

        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(5000);
            Console.WriteLine("Proceso A Finalizado");

        }
        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(5000);
            Console.WriteLine("Proceso B Finalizado");

        }
        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(5000);
            Console.WriteLine("Proceso C Finalizado");

        }
    }
}

