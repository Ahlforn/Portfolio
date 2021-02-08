using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using QuantumConcepts.Formats.StereoLithography;

namespace PriceCalculator.Models
{
    public class PrintModelFile
    {
        public enum FileType { STL };
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Volume { get; set; }

        public PrintModelFile(string file, FileType fileType)
        {
            // TODO: Possibly implement support for more formats

            STLDocument stl;

            try
            {
                Stream stream = File.OpenRead(file);
                stl = STLDocument.Read(stream, true);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            if(stl.Facets.Count == 0)
            {
                throw new Exception("STL file parsing failed.");
            }

            float maxX = float.MinValue, minX = float.MaxValue, maxY = float.MinValue, minY = float.MaxValue, maxZ = float.MinValue, minZ = float.MaxValue;
            bool first = true;

            foreach (Facet facet in stl.Facets)
            {
                foreach (Vertex v in facet.Vertices)
                {
                    if (v.X > maxX) maxX = v.X;
                    if (v.X < minX) minX = v.X;
                    if (v.Y > maxY) maxY = v.Y;
                    if (v.Y < minY) minY = v.Y;
                    if (v.Z > maxZ) maxZ = v.Z;
                    if (v.Z < minZ) minZ = v.Z;
                    if (first) first = false;
                }
            }

            this.X = Math.Round(maxX - minX, 2);
            this.Y = Math.Round(maxY - minY, 2);
            this.Z = Math.Round(maxZ - minZ, 2);

            // Calculate volume
            int f = stl.Facets.Count;
            double p = 0;

            foreach (Facet facet in stl.Facets)
            {
                double vAx = facet.Vertices[0].X;
                double vAy = facet.Vertices[0].Y;
                double vAz = facet.Vertices[0].Z;
                double vBx = facet.Vertices[1].X;
                double vBy = facet.Vertices[1].Y;
                double vBz = facet.Vertices[1].Z;
                double vCx = facet.Vertices[2].X;
                double vCy = facet.Vertices[2].Y;
                double vCz = facet.Vertices[2].Z;

                p += (-1 * vCx * vBy * vAz) + (vBx * vCy * vAz) + (vCx * vAy * vBz) - (vAx * vCy * vBz) - (vBx * vAy * vCz) + (vAx * vBy * vCz);
            }

            this.Volume = Math.Abs(p / 6);
        }
    }
}
