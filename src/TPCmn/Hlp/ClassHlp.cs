using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TPCmn {
	public static class ClassHlp {
		public static int _GetOrder(this PropertyInfo prp) {
			if (prp == null) {
				return -1;
			}
			var attr = prp.GetCustomAttributes(typeof(ExOrderAttribute),false);
			if ((attr == null) || (attr.Length < 1)) {
				return -1;
			}
			return ((ExOrderAttribute)attr[0]).ExOrder;
		}
		public static int _GetOrder(this FieldInfo fld) {
			if (fld == null) {
				return -1;
			}
			var attr = fld.GetCustomAttributes(typeof(ExOrderAttribute),false);
			if ((attr == null) || (attr.Length < 1)) {
				return -1;
			}
			return ((ExOrderAttribute)attr[0]).ExOrder;
		}
		public static bool _GetKey(this PropertyInfo prp) {
			if (prp == null) {
				return false;
			}
			var attr = prp.GetCustomAttributes(typeof(ExKeyAttribute),false);
			if ((attr == null) || (attr.Length < 1)) {
				return false;
			}
			return ((ExKeyAttribute)attr[0]).ExKey;
		}
		public static bool _GetKey(this FieldInfo fld) {
			if (fld == null) {
				return false;
			}
			var attr = fld.GetCustomAttributes(typeof(ExKeyAttribute),false);
			if ((attr == null) || (attr.Length < 1)) {
				return false;
			}
			return ((ExKeyAttribute)attr[0]).ExKey;
		}
		public static string _GetTable(this Type typ) {
			if (typ == null) {
				return null;
			}
			var attr = typ.GetCustomAttributes(typeof(ExTableAttribute),false);
			if ((attr == null) || (attr.Length < 1)) {
				return null;
			}
			return ((ExTableAttribute)attr[0]).ExTable;
		}

		private static readonly Dictionary<Type, List<MemberInfo>> typCache = new Dictionary<Type, List<MemberInfo>>();
		public static List<MemberInfo> _Defs(this Type typ) {
			if (typCache.ContainsKey(typ)) {
				return typCache[typ];
			}
			List<MemberInfo> lstRet = new List<MemberInfo>();
			List<MemberInfo> lstTmp = new List<MemberInfo>();
			var prps = typ.GetProperties();
			var flds = typ.GetFields();

			foreach (var prp in prps) {
				if (!prp.CanWrite) {
					continue;
				}
				lstTmp.Add(new MemberInfo { Name=prp.Name, Order=prp._GetOrder(), Typ=prp.PropertyType, fKey=prp._GetKey(), PrpInf = prp });
			}

			foreach (var fld in flds) {
				lstTmp.Add(new MemberInfo { Name=fld.Name, Order=fld._GetOrder(), Typ=fld.FieldType, fKey=fld._GetKey(), FldInf = fld });
			}

			typCache[typ] = lstTmp.OrderBy(x => x.Order).ToList();

			return typCache[typ];
		}
		public static List<MemberInfo> _Defs<T>(this Type typ, T tgt) {
			List<MemberInfo> lstRet = new List<MemberInfo>();
			List<MemberInfo> lstTmp = typ._Defs();

			foreach (MemberInfo mem in lstTmp) {
				MemberInfo memRet = new MemberInfo(mem);
				lstRet.Add(memRet);
				if (memRet.PrpInf is PropertyInfo prp) {
					if (!prp.CanWrite) {
						continue;
					}
					if (prp.PropertyType == typeof(int)) {
						memRet.fUseVal = true;
						memRet.ValI = (int)prp.GetValue(tgt);
						continue;
					}
					if (prp.PropertyType == typeof(int?)) {
						memRet.fUseVal = true;
						memRet.ValI = (int?)prp.GetValue(tgt);
						continue;
					}
					if (prp.PropertyType == typeof(decimal)) {
						memRet.fUseVal = true;
						memRet.ValN = (decimal)prp.GetValue(tgt);
						continue;
					}
					if (prp.PropertyType == typeof(decimal?)) {
						memRet.fUseVal = true;
						memRet.ValN = (decimal?)prp.GetValue(tgt);
						continue;
					}
					if (prp.PropertyType == typeof(string)) {
						memRet.fUseVal = true;
						memRet.ValS = (string)prp.GetValue(tgt);
						continue;
					}
					if (prp.PropertyType == typeof(DateTime)) {
						memRet.fUseVal = true;
						memRet.ValS = ((DateTime)prp.GetValue(tgt)).ToString("yyyy/MM/dd HH:mm:ss.fff");
						continue;
					}
					if (prp.PropertyType == typeof(string)) {
						memRet.fUseVal = true;
						memRet.ValS = ((DateTime?)prp.GetValue(tgt))?.ToString("yyyy/MM/dd HH:mm:ss.fff");
						continue;
					}
				}
				if (memRet.FldInf is FieldInfo fld) {
					if (fld.FieldType == typeof(int)) {
						memRet.fUseVal = true;
						memRet.ValI = (int)fld.GetValue(tgt);
						continue;
					}
					if (fld.FieldType == typeof(int?)) {
						memRet.fUseVal = true;
						memRet.ValI = (int?)fld.GetValue(tgt);
						continue;
					}
					if (fld.FieldType == typeof(decimal)) {
						memRet.fUseVal = true;
						memRet.ValN = (decimal)fld.GetValue(tgt);
						continue;
					}
					if (fld.FieldType == typeof(decimal?)) {
						memRet.fUseVal = true;
						memRet.ValN = (decimal?)fld.GetValue(tgt);
						continue;
					}
					if (fld.FieldType == typeof(string)) {
						memRet.fUseVal = true;
						memRet.ValS = (string)fld.GetValue(tgt);
						continue;
					}
					if (fld.FieldType == typeof(DateTime)) {
						memRet.fUseVal = true;
						memRet.ValS = ((DateTime)fld.GetValue(tgt)).ToString("yyyy/MM/dd HH:mm:ss.fff");
						continue;
					}
					if (fld.FieldType == typeof(DateTime?)) {
						memRet.fUseVal = true;
						memRet.ValS = ((DateTime?)fld.GetValue(tgt))?.ToString("yyyy/MM/dd HH:mm:ss.fff");
						continue;
					}
				}
			}
			return lstRet;
		}

		public static bool _KeyEql<T>(this Type typ, T src, T tgt) {
			List<MemberInfo> lstTmp = typ._Defs();

			foreach (MemberInfo mem in lstTmp) {
				if (!mem.fKey) {
					continue;
				}
				if (mem.PrpInf is PropertyInfo prp) {
					if (!prp.CanWrite) {
						continue;
					}
					if (prp.PropertyType == typeof(int) && (int)prp.GetValue(src)!=(int)prp.GetValue(tgt)) {
						return false;
					}
					if (prp.PropertyType == typeof(int?) && (int?)prp.GetValue(src)!=(int?)prp.GetValue(tgt)) {
						return false;
					}
					if (prp.PropertyType == typeof(decimal) && (decimal)prp.GetValue(src)!=(decimal)prp.GetValue(tgt)) {
						return false;
					}
					if (prp.PropertyType == typeof(decimal?) && (decimal?)prp.GetValue(src)!=(decimal?)prp.GetValue(tgt)) {
						return false;
					}
					if (prp.PropertyType == typeof(string) && (string)prp.GetValue(src)!=(string)prp.GetValue(tgt)) {
						return false;
					}
					if (prp.PropertyType == typeof(DateTime) && (DateTime)prp.GetValue(src)!=(DateTime)prp.GetValue(tgt)) {
						return false;
					}
					if (prp.PropertyType == typeof(DateTime?) && (DateTime?)prp.GetValue(src)!=(DateTime?)prp.GetValue(tgt)) {
						return false;
					}
				}
				if (mem.FldInf is FieldInfo fld) {
					if (fld.FieldType == typeof(int) && (int)fld.GetValue(src)!=(int)fld.GetValue(tgt)) {
						return false;
					}
					if (fld.FieldType == typeof(int?) && (int?)fld.GetValue(src)!=(int?)fld.GetValue(tgt)) {
						return false;
					}
					if (fld.FieldType == typeof(decimal) && (decimal)fld.GetValue(src)!=(decimal)fld.GetValue(tgt)) {
						return false;
					}
					if (fld.FieldType == typeof(decimal?) && (decimal?)fld.GetValue(src)!=(decimal?)fld.GetValue(tgt)) {
						return false;
					}
					if (fld.FieldType == typeof(string) && (string)fld.GetValue(src)!=(string)fld.GetValue(tgt)) {
						return false;
					}
					if (fld.FieldType == typeof(DateTime) && (DateTime)fld.GetValue(src)!=(DateTime)fld.GetValue(tgt)) {
						return false;
					}
					if (fld.FieldType == typeof(DateTime?) && (DateTime?)fld.GetValue(src)!=(DateTime?)fld.GetValue(tgt)) {
						return false;
					}
				}
			}
			return true;
		}

	}
	public class MemberInfo{
		public MemberInfo() { }
		public MemberInfo(MemberInfo from) {
			Name = from.Name;
			Order = from.Order;
			fKey = from.fKey;
			Typ = from.Typ;
			PrpInf = from.PrpInf;
			FldInf = from.FldInf;
		}

		public string Name;
		public int Order;
		public bool fKey = false;
		public Type Typ;
		public PropertyInfo PrpInf = null;
		public FieldInfo FldInf = null;
		public bool fUseVal = false;
		public string ValS;
		public int? ValI;
		public decimal? ValN;
	}




	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class ExTableAttribute : Attribute {
		private readonly string teble_;
		public ExTableAttribute(string _teble) {
			teble_ = _teble;
		}

		public string ExTable { get { return teble_; } }
	}

	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public class ExOrderAttribute : Attribute {
		private readonly int order_;
		public ExOrderAttribute([CallerLineNumber] int order = 0) {
			order_ = order;
		}

		public int ExOrder { get { return order_; } }
	}

	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public class ExKeyAttribute : Attribute {
		private readonly bool key_ = false;
		public ExKeyAttribute(bool _key = true) {
			key_ = _key;
		}

		public bool ExKey { get { return key_; } }
	}
}
