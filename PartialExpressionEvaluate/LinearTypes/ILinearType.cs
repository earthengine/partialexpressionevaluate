using System;
using System.Collections.Generic;
using System.Linq;

namespace PartialExpressionEvaluate.LinearTypes
{
    public interface ILinearType
    {
        ILinearType Negation();
        ILinearType TupleIndex(int index);
        ILinearType OptionIndex(int index);
        bool MatchEffects(IEnumerable<ILinearType> effects);
        bool MatchEnums(IEnumerable<ILinearType> branches);
    }

    public class StructType : ILinearType
    {
        List<ILinearType> items;

        public StructType(IEnumerable<ILinearType> items)
        {
            this.items = items.ToList();
        }

        public bool MatchEnums(IEnumerable<ILinearType> enums)
        {
            return enums.Count() == 1 && enums.First()==this;
        }

        public bool MatchEffects(IEnumerable<ILinearType> effects)
        {
            return effects.Count() == 1 && effects.First() == this;
        }

        public ILinearType Negation()
        {
            return new EffectsType(items.Select(i => i.Negation()));
        }

        public ILinearType OptionIndex(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException("index");
            return this;
        }

        public ILinearType TupleIndex(int index)
        {
            if (index < 0 || index >= items.Count) throw new ArgumentOutOfRangeException("index");
            return items[index];
        }
    }

    public class EffectsType : ILinearType
    {
        private List<ILinearType> effects;

        public EffectsType(IEnumerable<ILinearType> effects)
        {
            this.effects = effects.ToList();
        }

        public bool MatchEnums(IEnumerable<ILinearType> enums)
        {
            return enums.Count() == 1 && enums.First() == this;
        }

        public bool MatchEffects(IEnumerable<ILinearType> effects)
        {
            return effects.Count() == this.effects.Count && effects.Zip(this.effects, (e1, e2) => e1 == e2).Any(x => !x);
        }

        public ILinearType Negation()
        {
            return new StructType(effects.Select(e => e.Negation()));
        }

        public ILinearType OptionIndex(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException("index");
            return this;
        }

        public ILinearType TupleIndex(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException("index");
            return this;
        }
    }

    public class Optionstype : ILinearType
    {
        private readonly List<ILinearType> options;

        public bool MatchEnums(IEnumerable<ILinearType> enums)
        {
            return enums.Count() == 1 && enums.First() == this;
        }

        public bool MatchEffects(IEnumerable<ILinearType> effects)
        {
            return effects.Count() == 1 && effects.First() == this;
        }

        public ILinearType Negation()
        {
            return new EnumType(options.Select(o => o.Negation()));
        }

        public ILinearType OptionIndex(int index)
        {
            if (index < 0 || index >= options.Count) throw new ArgumentOutOfRangeException("index");
            return options[index];
        }

        public ILinearType TupleIndex(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException("index");
            return this;
        }
    }

    internal class EnumType : ILinearType
    {
        private IEnumerable<ILinearType> enums;

        public EnumType(IEnumerable<ILinearType> enums)
        {
            this.enums = enums;
        }

        public bool MatchEffects(IEnumerable<ILinearType> effects)
        {
            throw new NotImplementedException();
        }

        public bool MatchEnums(IEnumerable<ILinearType> branches)
        {
            throw new NotImplementedException();
        }

        public ILinearType Negation()
        {
            throw new NotImplementedException();
        }

        public ILinearType OptionIndex(int index)
        {
            throw new NotImplementedException();
        }

        public ILinearType TupleIndex(int index)
        {
            throw new NotImplementedException();
        }
    }
}
