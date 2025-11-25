using System;

namespace Ouroboros.Common.Utils.Math
{
    public struct PowerNum
    {
        public static PowerNum MaxValue = new PowerNum(int.MaxValue, double.MaxValue);
        public static PowerNum MinValue = new PowerNum(int.MinValue, double.MinValue);

        public int power;
        public double significant;

        public PowerNum(int power, double significand)
        {
            this.power = power;
            this.significant = significand;
        }

        public PowerNum(double value)
        {
            significant = value;
            power = 0;

            if (significant != 0)
            {
                Normalize();
            }
        }

        public static PowerNum operator +(PowerNum num1, PowerNum num2)
        {
            var total = new PowerNum();
            if (num1.power > num2.power)
            {
                total.power = num1.power;
                total.significant = num1.significant + num2.significant * System.Math.Pow(10, num2.power - num1.power);
            }
            else if (num1.power < num2.power)
            {
                total.power = num2.power;
                total.significant = num2.significant + num1.significant * System.Math.Pow(10, num1.power - num2.power);
            }
            else
            {
                total.power = num1.power;
                total.significant = num1.significant + num2.significant;
            }

            total.Normalize();

            return total;
        }

        public static PowerNum operator -(PowerNum num1, PowerNum num2)
        {
            var total = new PowerNum();
            if (num1.power > num2.power)
            {
                total.power = num1.power;
                total.significant = num1.significant - num2.significant * System.Math.Pow(10, num2.power - num1.power);
            }
            else if (num1.power < num2.power)
            {
                total.power = num2.power;
                total.significant = num1.significant * System.Math.Pow(10, num1.power - num2.power) - num2.significant;
            }
            else
            {
                total.power = num1.power;
                total.significant = num1.significant - num2.significant;
            }

            total.Normalize();

            return total;
        }

        public static PowerNum operator *(PowerNum num1, PowerNum num2)
        {
            var total = new PowerNum()
            {
                power = num1.power + num2.power,
                significant = num1.significant * num2.significant
            };

            total.Normalize();

            return total;
        }

        public static PowerNum operator /(PowerNum num1, PowerNum num2)
        {
            if (num2 == 0)
            {
                throw new DivideByZeroException();
            }

            var total = new PowerNum()
            {
                power = num1.power - num2.power,
                significant = num1.significant / num2.significant
            };

            total.Normalize();

            return total;
        }

        public static PowerNum operator *(PowerNum num, double f)
        {
            num.significant *= f;
            num.Normalize();

            return num;
        }

        public static PowerNum operator /(PowerNum num, double f)
        {
            if (f == 0) throw new DivideByZeroException();

            num.significant /= f;
            num.Normalize();

            return num;
        }

        public static PowerNum operator /(PowerNum num, float f)
        {
            if (f == 0) throw new DivideByZeroException();

            num.significant /= f;
            num.Normalize();

            return num;
        }

        public static bool operator <(PowerNum num1, PowerNum num2)
        {
            return Compare(num1, num2) < 0;
        }

        public static bool operator <=(PowerNum num1, PowerNum num2)
        {
            return Compare(num1, num2) <= 0;
        }

        public static bool operator >(PowerNum num1, PowerNum num2)
        {
            return Compare(num1, num2) > 0;
        }

        public static bool operator >=(PowerNum num1, PowerNum num2)
        {
            return Compare(num1, num2) >= 0;
        }

        public static bool operator ==(PowerNum num1, PowerNum num2)
        {
            return Compare(num1, num2) == 0;
        }

        public static bool operator !=(PowerNum num1, PowerNum num2)
        {
            return Compare(num1, num2) != 0;
        }

        public static bool operator ==(PowerNum num1, double d)
        {
            return Compare(num1, new PowerNum(d)) == 0;
        }

        public static bool operator !=(PowerNum num1, double d)
        {
            return Compare(num1, new PowerNum(d)) != 0;
        }

        private static int Compare(PowerNum num1, PowerNum num2)
        {
            int powerComp = num1.power.CompareTo(num2.power);
            if (powerComp == 0)
            {
                return num1.significant.CompareTo(num2.significant);
            }
            else
            {
                return powerComp;
            }
        }

        public static PowerNum Lerp(PowerNum num1, PowerNum num2, double factor)
        {
            var num = new PowerNum();
            num = num2 - num1;
            num = num1 + num * factor;

            num.Normalize();

            return num;
        }

        public static double Lerp(double d1, double d2, double factor)
        {
            return (d2 - d1) * factor + d1;
        }

        public static double InverseLerp(double d1, double d2, double current)
        {
            if (d1 == d2) return 1;

            return (current - d1) / (d2 - d1);
        }

        public static PowerNum PowerLerp(PowerNum num1, PowerNum num2, double factor)
        {
            var num = new PowerNum();

            var d1 = num1.ToLinear();
            var d2 = num2.ToLinear();

            var d = Lerp(d1, d2, factor);

            num.power = (int)d;
            num.significant = d - num.power;
            num.significant = num.significant * 9 + 1;
            num.Normalize();

            return num;
        }

        public static double InverseLerp(PowerNum num1, PowerNum num2, PowerNum current)
        {
            if (num1 == num2) return 1;

            return ((current - num1) / (num2 - num1)).ToDouble();
        }

        public static double InversePowerLerp(PowerNum num1, PowerNum num2, PowerNum current)
        {
            if (num1 == num2) return 1;

            var d1 = num1.ToLinear();
            var d2 = num2.ToLinear();
            var f = current.ToLinear();

            var factor = InverseLerp(d1, d2, f);

            return factor;
        }

        public double ToLinear()
        {
            return power + significant * 0.1;
        }

        public double ToDouble()
        {
            return System.Math.Pow(10, power) * significant;
        }

        public override string ToString()
        {
            return string.Format("{0:0.00}^{1}", significant, power);
        }

        public string ToString10xFormat()
        {
            return string.Format("{0:0.00} x 10^{1}", significant, power);
        }

        public void Normalize()
        {
            if (System.Math.Abs(significant) < double.Epsilon)
            {
                return;
            }

            int powerChange = 0;

            int iter = 0;
            const int iterMax = 100;

            if (significant > 0)
            {
                while (significant >= 10d && iter < iterMax)
                {
                    significant *= 0.1d;
                    ++powerChange;
                    ++iter;
                }

                while (significant < 1d && iter < iterMax)
                {
                    significant *= 10d;
                    --powerChange;
                    ++iter;
                }
            }
            else
            {
                while (significant <= -10d && iter < iterMax)
                {
                    significant *= 0.1d;
                    ++powerChange;
                    ++iter;
                }

                while (significant > -1d && iter < iterMax)
                {
                    significant *= 10d;
                    --powerChange;
                    ++iter;
                }
            }

            if (iter >= iterMax)
            {
                UnityEngine.Debug.LogError("PowerNum.Normalize() exceeded max iterations!");
            }

            power += powerChange;
        }

        public PowerNum ScaleByPower(int power)
        {
            return new PowerNum(this.power + power, significant);
        }

        public static bool AreSimilar(PowerNum num1, PowerNum num2, double precision = 0.0001)
        {
            if (num1.power != num2.power)
            {
                return false;
            }

            return System.Math.Abs(num1.significant - num2.significant) < precision;
        }

        public int RoundToPower()
        {
            Normalize();

            if (significant >= 5)
            {
                return power + 1;
            }

            if (significant < 0.5)
            {
                return power - 1;
            }

            return power;
        }
    }
}