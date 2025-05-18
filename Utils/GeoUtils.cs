using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba_Geolocalizacion.Utils
{
    public class GeoUtils
    {
        public static double CalcularDistancia(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double R = 6371e3; // radio de la Tierra en metros

            var φ1 = DegreesToRadians((double)lat1);
            var φ2 = DegreesToRadians((double)lat2);
            var Δφ = DegreesToRadians((double)(lat2 - lat1));
            var Δλ = DegreesToRadians((double)(lon2 - lon1));

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // en metros
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}