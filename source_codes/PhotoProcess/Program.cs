using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace PhotoProcess
{
    

    public class PhotoProcessor
    {
        private static int num_fixed_colors = 16;      // Determined from DSP
        private static int height = 160;                // Determined from DSP
        private static int width = 300;                 // Determined from DSP
        public static int max_iter;
        public static Color[] clusters;

        private string img_path;
        private int[,] assignment;


        public PhotoProcessor(string img_path_)
        {
            img_path = img_path_;
        }
        public void Process()
        {
            Bitmap bp = OpenAndResizeImage();
            Kmeans(ref bp);
            ShowKmean(ref bp);
            bp.Dispose();
            Save();
            Console.WriteLine("Done. 100%");
        }
        void Save()
        {
            Stream stream_assign = File.Open(@"out\assign.xml", FileMode.Create);
            BinaryFormatter formatter_assign = new BinaryFormatter();
            formatter_assign.Serialize(stream_assign, assignment);
            stream_assign.Close();

            Stream stream_cluster = File.Open(@"out\cluster.xml", FileMode.Create);
            BinaryFormatter formatter_cluster = new BinaryFormatter();
            formatter_cluster.Serialize(stream_cluster, clusters);
            stream_cluster.Close();
        }
        private void Kmeans(ref Bitmap bp)
        {
            assignment = new int[width, height];
            bool changed = true;
            int iter = 1;
            while (changed && iter <= max_iter)
            {
                Console.WriteLine(String.Format("iter {0} start. {1}%", iter, (iter - 1) * 100 / max_iter));
                changed = AssignClusters(ref bp);
                UpdateClusters(ref bp);
                iter++;
            }
            AssignClusters(ref bp);
        }
        bool AssignClusters(ref Bitmap bp)
        {
            bool changed = false;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int original_assignment = assignment[i, j];
                    assignment[i, j] = FindClosestColor(bp.GetPixel(i, j));
                    if (original_assignment != assignment[i, j])
                        changed = true;
                }
            }
            return changed;
        }
        void UpdateClusters(ref Bitmap bp)
        {
            // init RGB
            List<List<double>> R = new List<List<double>>();
            List<List<double>> G = new List<List<double>>();
            List<List<double>> B = new List<List<double>>();
            for (int i = 0; i < clusters.Length; i++)
            {
                R.Add(new List<double>());
                G.Add(new List<double>());
                B.Add(new List<double>());

            }
            for (int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    if (assignment[i, j] >= num_fixed_colors)
                    {
                        R[assignment[i, j]].Add((double)bp.GetPixel(i, j).R);
                        G[assignment[i, j]].Add((double)bp.GetPixel(i, j).G);
                        B[assignment[i, j]].Add((double)bp.GetPixel(i, j).B);
                    }
                }
            }

            for(int i = num_fixed_colors; i < clusters.Length; i++)
            {
                if (R[i].Count == 0)
                {
                    Random rnd = new Random();
                    clusters[i] = bp.GetPixel(rnd.Next(width), rnd.Next(height));
                }
                else
                {
                    clusters[i] = Color.FromArgb(255, (int)R[i].Average(), (int)G[i].Average(), (int)B[i].Average());
                }
            }
        }

        void ShowKmean(ref Bitmap bp)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color c = clusters[assignment[i, j]];
                    bp.SetPixel(i, j, Color.FromArgb(c.R, c.G, c.B));
                }
            }
            bp.Save(@"out\preview_final.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        int FindClosestColor(Color c1)
        {
            double min_distance =Double.MaxValue;
            int min_pos = 0;
            for (int i = 0; i < clusters.Length; i++)
            {
                Color c2 = clusters[i];
                double distance = Math.Sqrt((c1.R - c2.R)* (c1.R - c2.R) + (c1.G - c2.G)* (c1.G - c2.G) + (c1.B - c2.B) * (c1.B - c2.B));
                if (distance < min_distance)
                {
                    min_distance = distance;
                    min_pos = i;
                }
            }

            return min_pos;
        }
        /*
         * @return an Image specified by img_path_. The image is auto rotated resized to ensure  width and height.
         */
        private Bitmap OpenAndResizeImage()
        {
            Image image = Image.FromFile(img_path);
            if (image.Width < image.Height)
            {
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            destImage.Save(@"out\preview_resized.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return destImage;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid args. One file name plus one int expected.");
                return;
            }

            // init
            string file_name = args[0];
            PhotoProcessor.max_iter = int.Parse(args[1]);
            InitColors();
 
            // process
            PhotoProcessor pp = new PhotoProcessor(file_name);
            pp.Process();

        }
        static void InitColors()
        {
            PhotoProcessor.clusters = new Color[32]
            {
                    Color.FromArgb(0, 255, 255, 255),
                    Color.FromArgb(0, 216, 216, 216),
                    Color.FromArgb(0, 255, 0, 0),
                    Color.FromArgb(0, 255, 127, 0),
                    Color.FromArgb(0, 255, 255, 0),
                    Color.FromArgb(0, 127, 255, 0),
                    Color.FromArgb(0, 0, 255, 0),
                    Color.FromArgb(0, 0, 255, 127),
                    Color.FromArgb(0, 178, 178, 178),
                    Color.FromArgb(0, 140, 140, 140),
                    Color.FromArgb(0, 0, 255, 255),
                    Color.FromArgb(0, 0, 127, 255),
                    Color.FromArgb(0, 0, 0, 255),
                    Color.FromArgb(0, 127, 0, 255),
                    Color.FromArgb(0, 255, 0, 255),
                    Color.FromArgb(0, 255, 0, 127),
                    Color.FromArgb(0, 85, 25, 24),
                    Color.FromArgb(0, 235, 226, 223),
                    Color.FromArgb(0, 209, 159, 133),
                    Color.FromArgb(0, 126, 96, 113),
                    Color.FromArgb(0, 35, 47, 107),
                    Color.FromArgb(0, 87, 110, 185),
                    Color.FromArgb(0, 182, 113, 88),
                    Color.FromArgb(0, 165, 133, 148),
                    Color.FromArgb(0, 58, 80, 158),
                    Color.FromArgb(0, 16, 9, 12),
                    Color.FromArgb(0, 246, 241, 236),
                    Color.FromArgb(0, 36, 27, 46),
                    Color.FromArgb(0, 85, 60, 76),
                    Color.FromArgb(0, 161, 67, 56),
                    Color.FromArgb(0, 211, 189, 188),
                    Color.FromArgb(0, 140, 29, 29)
            };
        }
    }
}
