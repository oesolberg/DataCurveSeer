using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;


namespace DataCurve.Common
{


	public sealed class Classes
	{

		[Serializable]
		public class HsCollection : Dictionary<string, object>
		{
			private Collection<string> _keyIndex;

			//public new object Keys
			//{
			//    get
			//    {
			//        string result = null;
			//        checked
			//        {
			//            try
			//            {
			//                Dictionary<string, object>.KeyCollection.Enumerator enumerator = base.Keys.GetEnumerator();
			//                while (enumerator.MoveNext())
			//                {
			//                    result = enumerator.Current;
			//                    int num;
			//                    if (num == index)
			//                    {
			//                        break;
			//                    }
			//                    num++;
			//                }
			//            }
			//            finally
			//            {
			//                Dictionary<string, object>.KeyCollection.Enumerator enumerator;
			//                ((IDisposable)enumerator).Dispose();
			//            }
			//            return result;
			//        }
			//    }
			//}

			//public object this[int index]
			//{
			//    get
			//    {
			//        return base[Conversions.ToString(this._keyIndex[index])];
			//    }
			//    set
			//    {
			//        base[Conversions.ToString(this._keyIndex[index])] = RuntimeHelpers.GetObjectValue(value);
			//    }
			//}

			//public new object this[string key]
			//{
			//    get
			//    {
			//        int num;
			//        object result;
			//        int num3;
			//        try
			//        {
			//            IL_00:
			//            ProjectData.ClearProjectError();
			//            num = 1;
			//            IL_07:
			//            int num2 = 2;
			//            result = base[key];
			//            IL_11:
			//            goto IL_6C;
			//            IL_13:
			//            int arg_18_0 = num3 + 1;
			//            num3 = 0;
			//            @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], arg_18_0);
			//            IL_2D:
			//            goto IL_61;
			//            num3 = num2;
			//            @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num);
			//            IL_3F:
			//            goto IL_61;
			//        }

			//    object arg_41_0;
			//        endfilter(arg_41_0 is Exception & num != 0 & num3 == 0);
			//        IL_61:
			//        throw ProjectData.CreateProjectError(-2146828237);
			//        IL_6C:
			//        if (num3 != 0)
			//        {
			//            ProjectData.ClearProjectError();
			//        }
			//        return result;
			//    }
			//    set
			//    {
			//        if (!base.ContainsKey(key))
			//        {
			//            this.Add(RuntimeHelpers.GetObjectValue(value), key);
			//            return;
			//        }
			//        base[key] = RuntimeHelpers.GetObjectValue(value);
			//    }
			//}

			public HsCollection()
			{
				this._keyIndex = new Collection<string>();
			}

			protected HsCollection(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this._keyIndex = new Collection<string>();
			}

			public bool StartsWithKey(string key)
			{
				foreach (var collectionKey in base.Keys)
				{
					if (collectionKey.StartsWith(key))
					{
						return true;
					}
				}

				return false;
			}

			public void AddObject(string key, object value)
			{
				if (!base.ContainsKey(key))
				{
					base.Add(key, RuntimeHelpers.GetObjectValue(value));
					this._keyIndex.Add(key);
					return;
				}
				base[key] = RuntimeHelpers.GetObjectValue(value);
			}

			public bool KeyExists(string key)
			{
				return base.ContainsKey(key);
			}
			public bool KeyStartsWith(string keyStart)
			{
				foreach (var foundKey in base.Keys)
				{
					if (foundKey.StartsWith(keyStart))
					{
						return true;
					}
				}

				return false;
			}
			//public new void Remove(string key)
			//{
			//    base.Remove(_keyIndex(key));
			//    int num;
			//    int num3;
			//    try
			//    {
			//        IL_00:
			//        ProjectData.ClearProjectError();
			//        num = 1;
			//        IL_07:
			//        int num2 = 2;
			//        base.Remove(key);
			//        IL_11:
			//        num2 = 3;
			//        this._keyIndex.Remove(key);
			//        IL_1F:
			//        goto IL_7E;
			//        IL_21:
			//        int arg_26_0 = num3 + 1;
			//        num3 = 0;
			//        @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], arg_26_0);
			//        IL_3F:
			//        goto IL_73;
			//        num3 = num2;
			//        @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num);
			//        IL_51:
			//        goto IL_73;
			//    }

			//object arg_53_0;
			//    endfilter(arg_53_0 is Exception & num != 0 & num3 == 0);
			//    IL_73:
			//    throw ProjectData.CreateProjectError(-2146828237);
			//    IL_7E:
			//    if (num3 != 0)
			//    {
			//        ProjectData.ClearProjectError();
			//    }
			//}

			//public void Remove(int index)
			//{
			//    base.Remove(Conversions.ToString(this._keyIndex[index]));
			//    this._keyIndex.Remove(index);
			//}
		}

		[Serializable]
		public class action : Classes.HsCollection
		{
			public action()
			{
			}

			protected action(SerializationInfo info, StreamingContext context) : base(info, context)
			{
			}
		}

		[Serializable]
		public class trigger : Classes.HsCollection
		{
			public trigger()
			{
			}

			protected trigger(SerializationInfo info, StreamingContext context) : base(info, context)
			{
			}
		}
	}

}

