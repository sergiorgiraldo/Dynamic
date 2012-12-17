#pragma warning disable 168
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;

namespace DynTests
{
    internal class Program
    {
        private enum EPrecision
        {
            Ticks,
            Ms
        }

        public const int HowMany = 1000000;
        public const int Iterations = 1000;
        public static int Index;

        private static readonly List<long> DynamicTimes = new List<long>();
        private static readonly List<long> DirectTimes = new List<long>();

        private static void Main()
        {
            Console.WriteLine(HowMany + " values\n");
            Console.WriteLine(Iterations + " iterations\n");
            
            //Build values
            CfgDynamic.GetInstance();
            CfgDirect.GetInstance();

            DynamicTimes.Clear();
            for (var i = 1; i <= Iterations; i++)
            {
                DynamicTimes.Add(
                    Run("Dynamic -just one value-  no." + i.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'),
                        GetJustOneDynamic, EPrecision.Ms));
            }
            Console.WriteLine("Dynamic -just one value(ms)\n\tMédia:{0}, Máximo:{1}, Mínimo:{2}", DynamicTimes.Average(),
                              DynamicTimes.Max(),
                              DynamicTimes.Min());

            DirectTimes.Clear();
            for (var i = 1; i <= Iterations; i++)
            {
                DirectTimes.Add(
                    Run("Direct -just one value- no." + i.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'),
                        GetJustOneDirect, EPrecision.Ms));
            }
            Console.WriteLine("Direct -just one value(ms)\n\tMédia:{0}, Máximo:{1}, Mínimo:{2}", DirectTimes.Average(),
                              DirectTimes.Max(),
                              DirectTimes.Min());

            DynamicTimes.Clear();
            for (var i = 1; i <= Iterations; i++)
            {
                DynamicTimes.Add(
                    Run("Dynamic -all values-  no." + i.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'),
                        GetValuesDynamic, EPrecision.Ms));
            }
            Console.WriteLine("Dynamic -all values(ms)\n\tMédia:{0}, Máximo:{1}, Mínimo:{2}", DynamicTimes.Average(),
                              DynamicTimes.Max(),
                              DynamicTimes.Min());

            DirectTimes.Clear();
            for (var i = 1; i <= Iterations; i++)
            {
                DirectTimes.Add(Run(
                    "Direct -all values- no." + i.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'),
                    GetValuesDirect, EPrecision.Ms));
            }
            Console.WriteLine("Direct -all values(ms)\n\tMédia:{0}, Máximo:{1}, Mínimo:{2}", DirectTimes.Average(),
                              DirectTimes.Max(),
                              DirectTimes.Min());

            Console.ReadLine();
        }

        private static long Run(string log, Action run, EPrecision precision)
        {
            Stopwatch sw = Stopwatch.StartNew();
            run();
            sw.Stop();
            var elapsed = precision == EPrecision.Ms ? sw.ElapsedMilliseconds : sw.ElapsedTicks;
            //Console.WriteLine("{0}: {1}", log, elapsed);
            return elapsed;
        }

        private static void GetJustOneDirect()
        {
            CfgDirect cfgDirect = CfgDirect.GetInstance();
            string foo = cfgDirect.Vls[0].Foo;
            double bar = cfgDirect.Vls[0].Bar;
            DateTime baz = cfgDirect.Vls[0].Baz;
        }

        private static void GetValuesDirect()
        {
            CfgDirect cfgDirect = CfgDirect.GetInstance();
            for (int i = 0; i < HowMany; i++)
            {
                Index = i;
                string foo = cfgDirect.Vls[Index].Foo;
                double bar = cfgDirect.Vls[Index].Bar;
                DateTime baz = cfgDirect.Vls[Index].Baz;
                Index ++;
            }
        }

        private static void GetJustOneDynamic()
        {
            dynamic cfgDynamic = CfgDynamic.GetInstance();
            Index = 0;
            string foo = cfgDynamic.Foo;
            double bar = cfgDynamic.Bar;
            DateTime baz = cfgDynamic.Baz;
        }

        private static void GetValuesDynamic()
        {
            dynamic cfgDynamic = CfgDynamic.GetInstance();
            for (int i = 0; i < HowMany; i++)
            {
                Index = i;
                string foo = cfgDynamic.Foo;
                double bar = cfgDynamic.Bar;
                DateTime baz = cfgDynamic.Baz;
                Index ++;
            }
        }
    }

    internal class CfgDynamic : DynamicObject
    {
        public static readonly CfgDynamic Instance = new CfgDynamic();
        private readonly List<Values> _vls;

        private CfgDynamic()
        {
            _vls = new List<Values>();
            _vls.AddRange(Enumerable.Repeat(new Values(), Program.HowMany));
        }

        public static dynamic GetInstance()
        {
            return Instance;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name.Equals("Foo"))
            {
                result = _vls[Program.Index].Foo;
                return true;
            }
            if (binder.Name.Equals("Bar"))
            {
                result = _vls[Program.Index].Bar;
                return true;
            }
            if (binder.Name.Equals("Baz"))
            {
                result = _vls[Program.Index].Baz;
                return true;
            }
            result = null;
            return false;
        }
    }

    internal class CfgDirect
    {
        private static readonly CfgDirect Instance = new CfgDirect();
        public List<Values> Vls;

        private CfgDirect()
        {
            Vls = new List<Values>();
            Vls.AddRange(Enumerable.Repeat(new Values(), Program.HowMany));
        }

        public static CfgDirect GetInstance()
        {
            return Instance;
        }
    }

    internal class Values
    {
        public string Foo
        {
            get { return "Dummy"; }
        }

        public double Bar
        {
            get { return 42.05; }
        }

        public DateTime Baz
        {
            get { return new DateTime(); }
        }
    }
}

#pragma warning restore 168
