using System;
using Axiom.Math;
namespace Axiom.Hydrax
{
	/// <summary>
	/// Size structs used for water creation.
	/// </summary>
	public struct Size
    {
        /// Width value
        public int Width;
        /// Height value
        public int Height;


        public Size(int size)
        {
            Width  = size;
            Height = size;
        }


        public Size(int width, int height)
        {
            Width  = width;
            Height = height;
        }


        public void SetSize(int size)
        {
            Width  = size;
            Height = size;
        }

        public void SetSize(int width, int height)
        {
            Width  = width;
            Height = height;
        }
    }
	
    public class MathHelper
    {
        public class Pair<T, U>
        {
            public Pair()
            {
            }

            public Pair(T first, U second)
            {
                this.First = first;
                this.Second = second;
            }

            public T First { get; set; }
            public U Second { get; set; }
        };

        [Obsolete]
        public static bool IntersectionOfTwoLines(Vector2 a, Vector2 b, Vector2 c,
                                           Vector2 d, ref Vector2 result)
        {
            double r, s;

            double denominator = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);

            // If the denominator in above is zero, AB & CD are colinear
            if (denominator == 0)
                return false;

            double numeratorR = (a.y - c.y) * (d.x - c.x) - (a.x - c.x) * (d.y - c.y);
            //  If the numerator above is also zero, AB & CD are collinear.
            //  If they are collinear, then the segments may be projected to the x- 
            //  or y-axis, and overlap of the projected intervals checked.

            r = numeratorR / denominator;

            double numeratorS = (a.y - c.y) * (b.x - a.x) - (a.x - c.x) * (b.y - a.y);

            s = numeratorS / denominator;

            //  If 0<=r<=1 & 0<=s<=1, intersection exists
            //  r<0 or r>1 or s<0 or s>1 line segments do not intersect
            if (r < 0 || r > 1 || s < 0 || s > 1)
                return false;

            ///*
            //    Note:
            //    If the intersection point of the 2 lines are needed (lines in this
            //    context mean infinite lines) regardless whether the two line
            //    segments intersect, then
            //
            //        If r>1, P is located on extension of AB
            //        If r<0, P is located on extension of BA
            //        If s>1, P is located on extension of CD
            //        If s<0, P is located on extension of DC
            //*/

            // Find intersection point
            result.x = (float)(a.x + (r * (b.x - a.x)));
            result.y = (float)(a.y + (r * (b.y - a.y)));

            return true;
        }

        public static Vector2 IntersectionOfTwoLines(Vector2 a, Vector2 b,
         Vector2 c, Vector2 d)
        {
            float r, s, denominator = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);

            // If the denominator in above is zero, AB & CD are colinear
            if (denominator == 0)
            {
                return new Vector2(0, 0);
            }

            float numeratorR = (a.y - c.y) * (d.x - c.x) - (a.x - c.x) * (d.y - c.y);
            //  If the numerator above is also zero, AB & CD are collinear.
            //  If they are collinear, then the segments may be projected to the x- 
            //  or y-axis, and overlap of the projected intervals checked.

            r = numeratorR / denominator;

            float numeratorS = (a.y - c.y) * (b.x - a.x) - (a.x - c.x) * (b.y - a.y);

            s = numeratorS / denominator;

            //  If 0<=r<=1 & 0<=s<=1, intersection exists
            //  r<0 or r>1 or s<0 or s>1 line segments do not intersect
            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return new Vector2(0, 0);
            }

            ///*
            //    Note:
            //    If the intersection point of the 2 lines are needed (lines in this
            //    context mean infinite lines) regardless whether the two line
            //    segments intersect, then
            //
            //        If r>1, P is located on extension of AB
            //        If r<0, P is located on extension of BA
            //        If s>1, P is located on extension of CD
            //        If s<0, P is located on extension of DC
            //*/

            // Find intersection point
            return new Vector2((a.x + (r * (b.x - a.x))),
                                 (a.y + (r * (b.y - a.y))));
        }
        [Obsolete]
        public static float Vector2Length(Vector2 RefVector)
        {
            float result;
            Vector2 zeroVector = new Vector2(0, 0);
            DistanceSquared(ref RefVector, ref zeroVector, out result);
            return (float)System.Math.Sqrt(result);

        }
        public static void DistanceSquared(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            result = (value1.x - value2.x) * (value1.x - value2.x) + (value1.y - value2.y) * (value1.y - value2.y);
        }

        public struct Complex
        {
            float real;
            float imaginary;

            public Complex(float real, float imaginary)
            {
                this.real = real;
                this.imaginary = imaginary;
            }

            public float Real
            {
                get
                {
                    return (real);
                }
                set
                {
                    real = value;
                }
            }

            public float Imaginary
            {
                get
                {
                    return (imaginary);
                }
                set
                {
                    imaginary = value;
                }
            }

            public override string ToString()
            {
                return (String.Format("({0}, {1}i)", real, imaginary));
            }

            public static bool operator ==(Complex c1, Complex c2)
            {
                if ((c1.real == c2.real) &&
                (c1.imaginary == c2.imaginary))
                    return (true);
                else
                    return (false);
            }

            public static bool operator !=(Complex c1, Complex c2)
            {
                return (!(c1 == c2));
            }

            public override bool Equals(object o2)
            {
                Complex c2 = (Complex)o2;

                return (this == c2);
            }

            public override int GetHashCode()
            {
                return (real.GetHashCode() ^ imaginary.GetHashCode());
            }

            public static Complex operator +(Complex c1, Complex c2)
            {
                return (new Complex(c1.real + c2.real, c1.imaginary + c2.imaginary));
            }

            public static Complex operator -(Complex c1, Complex c2)
            {
                return (new Complex(c1.real - c2.real, c1.imaginary - c2.imaginary));
            }

            // product of two complex numbers
            public static Complex operator *(Complex c1, Complex c2)
            {
                return (new Complex(c1.real * c2.real - c1.imaginary * c2.imaginary,
                c1.real * c2.imaginary + c2.real * c1.imaginary));
            }

            // quotient of two complex numbers
            public static Complex operator /(Complex c1, Complex c2)
            {
                if ((c2.real == 0.0f) &&
                (c2.imaginary == 0.0f))
                    throw new DivideByZeroException("Can't divide by zero Complex number");

                float newReal =
                (c1.real * c2.real + c1.imaginary * c2.imaginary) /
                (c2.real * c2.real + c2.imaginary * c2.imaginary);
                float newImaginary =
                (c2.real * c1.imaginary - c1.real * c2.imaginary) /
                (c2.real * c2.real + c2.imaginary * c2.imaginary);

                return (new Complex(newReal, newImaginary));
            }

            // non-operator versions for other languages
            public static Complex Add(Complex c1, Complex c2)
            {
                return (c1 + c2);
            }

            public static Complex Subtract(Complex c1, Complex c2)
            {
                return (c1 - c2);
            }

            public static Complex Multiply(Complex c1, Complex c2)
            {
                return (c1 * c2);
            }

            public static Complex Divide(Complex c1, Complex c2)
            {
                return (c1 / c2);
            }
        }
        /// <summary>
        /// Random number generator.
        /// </summary>
        static Random _random = new Random();

        /// <summary>
        /// Get the random number generator.
        /// </summary>
        public static Random Random
        {
            get
            {
                return _random;
            }
        }
        public static float Clamp(float value, float min, float max)
        {
            // First we check to see if we're greater than the max
            value = (value > max) ? max : value;

            // Then we check to see if we're less than the min.
            value = (value < min) ? min : value;

            // There's no check to see if min > max.
            return value;
        }
        public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            // All transformed to double not to lose precission
            // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
            double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
            double sCubed = s * s * s;
            double sSquared = s * s;

            if (amount == 0f)
                result = value1;
            else if (amount == 1f)
                result = value2;
            else
                result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                    (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                    t1 * s +
                    v1;
            return (float)result;
        }

        public static float SmoothStep(float value1, float value2, float amount)
        {
            // It is expected that 0 < amount < 1
            // If amount < 0, return value1
            // If amount > 1, return value2
            float result = Clamp(amount, 0f, 1f);
            result = Hermite(value1, 0f, value2, 0f, result);
            return result;
        }

        /// <summary>
        /// Return a float between min and max.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float GetFloatInRange(float min, float max)
        {
            return min + (max - min) * (float)_random.NextDouble();
        }

        /// <summary>
        /// Return some noise for a specific seed.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static float Noise(int i)
        {
            i = (i << 13) ^ i;

            return (1.0f - ((i * (i * i * 15731 + 789221) + 1376312589) & 0x7FFFFFFF) / 1073741824.0f);
        }

    }
    public static class Extensions
    {
        
        public static Vector2 ToNormalized(this Vector2 Vector)
        {
            Normalize(Vector);
            return Vector;
        }
        public static Vector2 Perpendicular(this Vector2 Vector)
        {
            return new Vector2(-Vector.y, Vector.x);
        }
        private static float Normalize(Vector2 Vector)
        {
            float length = Utility.Sqrt(Vector.x * Vector.x + Vector.y * Vector.y);

            // Will also work for zero-sized vectors, but will change nothing
            if (length > float.Epsilon)
            {
                float inverseLength = 1.0f / length;

                Vector.x *= inverseLength;
                Vector.y *= inverseLength;
            }

            return length;
        }
        public static Vector2 Multiply(this Vector2 Vector, float Factor)
        {
            Vector2 result = new Vector2();
            result.x = Vector.x * Factor;
            result.y = Vector.y * Factor;
            return result;
        }
        public static float Length(this Vector2 Vector)
        {
            return (float)System.Math.Sqrt(Vector.x * Vector.x + Vector.y * Vector.y);
        }
    }
}
