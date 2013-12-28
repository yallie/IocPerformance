using System;
using System.Collections.Generic;
using IocPerformance.Classes.Complex;
using IocPerformance.Classes.Conditions;
using IocPerformance.Classes.Dummy;
using IocPerformance.Classes.Generics;
using IocPerformance.Classes.Multiple;
using IocPerformance.Classes.Properties;
using IocPerformance.Classes.Standard;

namespace IocPerformance.Adapters
{
    public sealed class NoneContainerAdapter : ContainerAdapterBase
    {
        private ArrayDictionary<Func<object>> container = new ArrayDictionary<Func<object>>();

        private class ArrayDictionary<V>
        {
        	private const int ArraySize = 1 << 8; // 256

        	private const int SizeMask = ArraySize - 1; // 0xFF

        	private Dictionary<Type, V> sourceValues = new Dictionary<Type, V>();

        	private V[] mappedValues = new V[ArraySize];

        	private int shift;

        	public V this[Type t]
        	{
        		set
        		{
        			sourceValues[t] = value;
        			shift = RemapValuesAndFindShiftParameter();
        		}
        		get
        		{
        			return mappedValues[(t.GetHashCode() >> shift) & SizeMask];
        		}
        	}

        	private int RemapValuesAndFindShiftParameter()
        	{
        		for (int i = 0; i < 32; i++)
        		{
        			var keys = new HashSet<int>();
        			foreach (var type in sourceValues.Keys)
        			{
        				var key = (type.GetHashCode() >> i) & SizeMask;
        				keys.Add(key);
        				mappedValues[key] = sourceValues[type];
        			}

        			if (keys.Count == sourceValues.Count)
        			{
        				// found the proper shift which yields unique indexes
        				return i;
        			}
        		}

        		throw new InvalidOperationException("Cannot find shift parameter yielding unique indexes.");
        	}
        }

        public override string PackageName
        {
            get { return "None"; }
        }

        public override string Url
        {
            get { return string.Empty; }
        }

        public override string Version
        {
            get { return string.Empty; }
        }

        public override bool SupportsConditional
        {
            get { return true; }
        }

        public override bool SupportGeneric
        {
            get { return true; }
        }

        public override bool SupportsMultiple
        {
            get { return true; }
        }

        public override bool SupportsPropertyInjection
        {
            get { return true; }
        }

        public override object Resolve(Type type)
        {
            return this.container[type]();
        }

        public override void Dispose()
        {
        }

        public override void Prepare()
        {
            this.RegisterDummies();
            this.RegisterStandard();
            this.RegisterComplex();
            this.RegisterPropertyInjection();
            this.RegisterOpenGeneric();
            this.RegisterConditional();
            this.RegisterMultiple();
        }

        private void RegisterDummies()
        {
            this.container[typeof(IDummyOne)] = () => new DummyOne();
            this.container[typeof(IDummyTwo)] = () => new DummyTwo();
            this.container[typeof(IDummyThree)] = () => new DummyThree();
            this.container[typeof(IDummyFour)] = () => new DummyFour();
            this.container[typeof(IDummyFive)] = () => new DummyFive();
            this.container[typeof(IDummySix)] = () => new DummySix();
            this.container[typeof(IDummySeven)] = () => new DummySeven();
            this.container[typeof(IDummyEight)] = () => new DummyEight();
            this.container[typeof(IDummyNine)] = () => new DummyNine();
            this.container[typeof(IDummyTen)] = () => new DummyTen();
        }

        private void RegisterStandard()
        {
            ISingleton singleton = new Singleton();

            this.container[typeof(ISingleton)] = () => singleton;
            this.container[typeof(ITransient)] = () => new Transient();
            this.container[typeof(ICombined)] = () => new Combined(singleton, new Transient());
        }

        private void RegisterComplex()
        {
            IFirstService firstService = new FirstService();
            ISecondService secondService = new SecondService();
            IThirdService thirdService = new ThirdService();

            this.container[typeof(IFirstService)] = () => firstService;
            this.container[typeof(ISecondService)] = () => secondService;
            this.container[typeof(IThirdService)] = () => thirdService;
            this.container[typeof(IComplex)] = () => new Complex(
                 firstService,
                 secondService,
                 thirdService,
                 new SubObjectOne(firstService),
                 new SubObjectTwo(secondService),
                 new SubObjectThree(thirdService));
        }

        private void RegisterPropertyInjection()
        {
            IServiceA serviceA = new ServiceA();
            IServiceB serviceB = new ServiceB();
            IServiceC serviceC = new ServiceC();

            this.container[typeof(IComplexPropertyObject)] = () =>
                new ComplexPropertyObject
                {
                    ServiceA = serviceA,
                    ServiceB = serviceB,
                    ServiceC = serviceC,
                    SubObjectA = new SubObjectA { ServiceA = serviceA },
                    SubObjectB = new SubObjectB { ServiceB = serviceB },
                    SubObjectC = new SubObjectC { ServiceC = serviceC }
                };
        }

        private void RegisterOpenGeneric()
        {
            this.container[typeof(ImportGeneric<int>)] =
                () => new ImportGeneric<int>(new GenericExport<int>());
        }

        private void RegisterConditional()
        {
            this.container[typeof(ImportConditionObject)] =
                () => new ImportConditionObject(new ExportConditionalObject());

            this.container[typeof(ImportConditionObject2)] =
                () => new ImportConditionObject2(new ExportConditionalObject2());
        }

        private void RegisterMultiple()
        {
            var adapters = GetAllSimpleAdapters();

            this.container[typeof(ImportMultiple)] = () => new ImportMultiple(adapters);
        }

        private static IEnumerable<ISimpleAdapter> GetAllSimpleAdapters()
        {
            yield return new SimpleAdapterOne();
            yield return new SimpleAdapterTwo();
            yield return new SimpleAdapterThree();
            yield return new SimpleAdapterFour();
            yield return new SimpleAdapterFive();
        }
    }
}