using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace ByteSheep.Events
{
	public enum AdvancedSupportedTypes { Void = 0, Object, String, Int, Float, Bool, Color, Vector2, Vector3, Enum }
	
	// TODO:
	// • Enum support
	// • Refactoring
	
    [Serializable]
	public class AdvancedArgumentCache
	{
		public AdvancedSupportedTypes m_supportedType = AdvancedSupportedTypes.Void;
		public Object m_objectArgument;
		public string m_stringArgument;
		public int m_intArgument;
		public float m_floatArgument;
		public bool m_boolArgument;
		public Color m_colorArgument;
		public Vector2 m_vector2Argument;
		public Vector3 m_vector3Argument;
		
		public object GetArgumentValue ()
		{
			switch (m_supportedType)
			{
				case AdvancedSupportedTypes.Object:
					return m_objectArgument;
				case AdvancedSupportedTypes.String:
					return m_stringArgument;
				case AdvancedSupportedTypes.Int:
					return m_intArgument;
				case AdvancedSupportedTypes.Float:
					return m_floatArgument;
				case AdvancedSupportedTypes.Bool:
					return m_boolArgument;
				case AdvancedSupportedTypes.Color:
					return m_colorArgument;
				case AdvancedSupportedTypes.Vector2:
					return m_vector2Argument;
				case AdvancedSupportedTypes.Vector3:
					return m_vector3Argument;
				default:
					return null;
			}
		}
		
		public Type GetArgumentType ()
		{
			switch (m_supportedType)
			{
				case AdvancedSupportedTypes.Object:
					return typeof (Object);
				case AdvancedSupportedTypes.String:
					return typeof (string);
				case AdvancedSupportedTypes.Int:
					return typeof (int);
				case AdvancedSupportedTypes.Float:
					return typeof (float);
				case AdvancedSupportedTypes.Bool:
					return typeof (bool);
				case AdvancedSupportedTypes.Color:
					return typeof (Color);
				case AdvancedSupportedTypes.Vector2:
					return typeof (Vector2);
				case AdvancedSupportedTypes.Vector3:
					return typeof (Vector3);
				default:
					return null;
			}
		}
		
		public static object[] CombineArguments (AdvancedArgumentCache[] _arguments)
		{
			object[] argumentValues = new object[_arguments.Length];
			for (int i = 0; i < argumentValues.Length; i++)
				argumentValues[i] = _arguments[i].GetArgumentValue ();
			return argumentValues;
		}
		
		public static Type[] CombineArgumentTypes (AdvancedArgumentCache[] _arguments)
		{
			Type[] argumentTypes = new Type[_arguments.Length];
			for (int i = 0; i < argumentTypes.Length; i++)
				argumentTypes[i] = _arguments[i].GetArgumentValue ().GetType ();
			return argumentTypes;
		}
	}
	
	public class DynamicArguments
	{
		private object[] m_oneArgument = new object[1];
		private object[] m_twoArguments = new object[2];
		private object[] m_threeArguments = new object[3];
		private object[] m_fourArguments = new object[4];
		private int m_argumentCount = 1;
		
		// Separated parameters avoid the cost of creating a new object array each time the event is invoked
		public object[] UpdateDynamicArguments (object _arg0, object _arg1 = null, object _arg2 = null, object _arg3 = null)
		{
			m_argumentCount = 1;
			if (_arg1 != null) m_argumentCount++;
			if (_arg2 != null) m_argumentCount++;
			if (_arg3 != null) m_argumentCount++;
			
			switch (m_argumentCount)
			{
				case 1:
				m_oneArgument[0] = _arg0;
				return m_oneArgument;
				case 2:
				m_twoArguments[0] = _arg0;
				m_twoArguments[1] = _arg1;
				return m_twoArguments;
				case 3:
				m_threeArguments[0] = _arg0;
				m_threeArguments[1] = _arg1;
				m_threeArguments[2] = _arg1;
				return m_threeArguments;
				case 4:
				m_fourArguments[0] = _arg0;
				m_fourArguments[1] = _arg1;
				m_fourArguments[2] = _arg2;
				m_fourArguments[3] = _arg3;
				return m_fourArguments;
				default:
				return null;
			}
		}
	}
	
	[Serializable]
	public class AdvancedPersistentCallGroup
	{
		public List<AdvancedPersistentCall> m_calls = new List<AdvancedPersistentCall> ();
	}
	
	[Serializable]
	public class AdvancedPersistentCall
	{
		public GenericMenuData m_genericMenuData;
		public Object m_target;
		public string m_memberName;
		public MemberTypes m_memberType;
		public FieldInfo m_fieldInfo;
		public MethodInfo m_methodInfo;
		public QuickAction ZeroParamMethod = delegate {};
		public AdvancedArgumentCache[] m_arguments;
		public object[] m_argumentValues;
		public Type[] m_argumentTypes;
		public bool m_isDynamic;
		public bool m_isCallEnabled;
		
		public void Invoke ()
		{
			if (!m_isCallEnabled || m_target == null) return;
			
			if (m_memberType == MemberTypes.Field && m_argumentValues.Length > 0 && m_fieldInfo != null)
				m_fieldInfo.SetValue (m_target, m_argumentValues[0]);
			else
			{
				if (m_argumentValues.Length == 0)
					ZeroParamMethod ();
				else if (m_methodInfo != null)
					m_methodInfo.Invoke (m_target, m_argumentValues);
			}
		}
		
		public void SetDynamicArguments (object[] _dynamicArguments)
		{
			if (m_isCallEnabled && m_isDynamic)
				m_argumentValues = _dynamicArguments;
		}
	}
	
	[Serializable]
	public abstract class AdvancedEventBase : ISerializationCallbackReceiver
	{
		#pragma warning disable 0414
		[SerializeField] private float m_inspectorListHeight = 40f;
		#pragma warning restore 0414
		public AdvancedPersistentCallGroup m_persistentCalls = new AdvancedPersistentCallGroup ();
		private DynamicArguments m_dynamicArguments = new DynamicArguments ();
		
		protected void InvokePersistent ()
		{
			for (int i = 0; i < m_persistentCalls.m_calls.Count; i++)
				m_persistentCalls.m_calls[i].Invoke ();
		}
		
		// Update the argument value of each dynamic call, with the argument passed to AdvancedEvent<T>.Invoke (T _arg0);
		protected void InvokePersistent (object _arg0, object _arg1 = null, object _arg2 = null, object _arg3 = null)
		{
			for (int i = 0; i < m_persistentCalls.m_calls.Count; i++)
			{
				m_persistentCalls.m_calls[i].SetDynamicArguments (m_dynamicArguments.UpdateDynamicArguments (_arg0, _arg1, _arg2, _arg3));
				m_persistentCalls.m_calls[i].Invoke ();
			}
		}
		
		/// <summary>Get the number of registered persistent listeners.</summary>
		public int GetPersistentEventCount ()
		{
			return m_persistentCalls.m_calls.Count;
		}
		
		/// <summary>Get the target member name of the listener at index.</summary>
		/// <param name="_index">Index of the listener to query.</param>
		public string GetPersistentMemberName (int _index)
		{
			return (GetPersistentEventCount () > 0) ? m_persistentCalls.m_calls[Mathf.Clamp (_index, 0, Mathf.Max (0, GetPersistentEventCount () - 1))].m_memberName : null;
		}
		
		/// <summary>Get the target component of the listener at index.</summary>
		/// <param name="_index">Index of the listener to query.</param>
		public Object GetPersistentTarget (int _index)
		{
			return (GetPersistentEventCount () > 0) ? m_persistentCalls.m_calls[Mathf.Clamp (_index, 0, Mathf.Max (0, GetPersistentEventCount () - 1))].m_target : null;
		}
		
		/// <summary>Modify the execution state of a listener.</summary>
		/// <param name="_index">Index of the listener to query.</param>
		/// <param name="_enabled">State to set.</param>
		public void SetPersistentListenerState (int _index, bool _enabled)
		{
			if (GetPersistentEventCount () > 0)
			{
				m_persistentCalls.m_calls[Mathf.Clamp (_index, 0, Mathf.Max (0, GetPersistentEventCount () - 1))].m_isCallEnabled = _enabled;
				if (_enabled)
					this.OnAfterDeserialize ();
			}
		}
		
		public virtual void OnBeforeSerialize () {}
		public virtual void OnAfterDeserialize ()
		{
			for (int i = 0; i < m_persistentCalls.m_calls.Count; i++)
			{
				AdvancedPersistentCall persistentCall = m_persistentCalls.m_calls[i];
				
				if (!persistentCall.m_isCallEnabled || persistentCall.m_target == null || persistentCall.m_memberName == "")
					continue;
				
				persistentCall.m_argumentValues = AdvancedArgumentCache.CombineArguments (persistentCall.m_arguments);
				persistentCall.m_argumentTypes = AdvancedArgumentCache.CombineArgumentTypes (persistentCall.m_arguments);
				
				if (persistentCall.m_memberType == MemberTypes.Method)
				{
					persistentCall.m_methodInfo = persistentCall.m_target.GetType ().GetMethod (persistentCall.m_memberName, persistentCall.m_argumentTypes);
					if (persistentCall.m_argumentTypes.Length == 0)
						persistentCall.ZeroParamMethod = (QuickAction) Delegate.CreateDelegate (typeof (QuickAction), persistentCall.m_target, persistentCall.m_methodInfo, true);
				}
				else if (persistentCall.m_memberType == MemberTypes.Property)
				{
					PropertyInfo propertyInfo = persistentCall.m_target.GetType ().GetProperty (persistentCall.m_memberName);
					if (propertyInfo != null)
						persistentCall.m_methodInfo = propertyInfo.GetSetMethod ();
				}
				else if (persistentCall.m_memberType == MemberTypes.Field)
				{
					persistentCall.m_fieldInfo = persistentCall.m_target.GetType ().GetField (persistentCall.m_memberName);
				}
			}
		}
	}
	
	[Serializable]
	public class AdvancedEvent : AdvancedEventBase
	{
		protected QuickAction DynamicMethodCalls;
		
		/// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
		public void Invoke ()
		{
			if (DynamicMethodCalls != null)
				DynamicMethodCalls ();
			
			base.InvokePersistent ();
		}
		
		/// <summary>Add a non persistent listener to the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void AddListener (QuickAction _listener) { DynamicMethodCalls += _listener; }
		/// <summary>Remove a non persistent listener from the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void RemoveListener (QuickAction _listener) { DynamicMethodCalls -= _listener; }
		/// <summary>Remove all non persistent listeners from the event.</summary>
		public void RemoveAllListeners () { DynamicMethodCalls = null; }
	}
	
	[Serializable]
	public class AdvancedEvent<T> : AdvancedEventBase
	{
		protected QuickAction<T> DynamicMethodCalls;
		
		/// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
		public void Invoke (T _arg0)
		{
			if (DynamicMethodCalls != null)
				DynamicMethodCalls (_arg0);
			
			base.InvokePersistent (_arg0);
		}
		
		protected Type GetActionType () { return typeof (QuickAction<T>); }
		/// <summary>Add a non persistent listener to the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void AddListener (QuickAction<T> _listener) { DynamicMethodCalls += _listener; }
		/// <summary>Remove a non persistent listener from the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void RemoveListener (QuickAction<T> _listener) { DynamicMethodCalls -= _listener; }
		/// <summary>Remove all non persistent listeners from the event.</summary>
		public void RemoveAllListeners () { DynamicMethodCalls = null; }
	}
	
	[Serializable]
	public class AdvancedEvent<T, U> : AdvancedEventBase
	{
		protected QuickAction<T, U> DynamicMethodCalls;
		
		/// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
		public void Invoke (T _arg0, U _arg1)
		{
			if (DynamicMethodCalls != null)
				DynamicMethodCalls (_arg0, _arg1);
			
			base.InvokePersistent (_arg0, _arg1);
		}
		
		protected Type GetActionType () { return typeof (QuickAction<T, U>); }
		/// <summary>Add a non persistent listener to the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void AddListener (QuickAction<T, U> _listener) { DynamicMethodCalls += _listener; }
		/// <summary>Remove a non persistent listener from the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void RemoveListener (QuickAction<T, U> _listener) { DynamicMethodCalls -= _listener; }
		/// <summary>Remove all non persistent listeners from the event.</summary>
		public void RemoveAllListeners () { DynamicMethodCalls = null; }
	}
	
	[Serializable]
	public class AdvancedEvent<T, U, V> : AdvancedEventBase
	{
		protected QuickAction<T, U, V> DynamicMethodCalls;
		
		/// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
		public void Invoke (T _arg0, U _arg1, V _arg2)
		{
			if (DynamicMethodCalls != null)
				DynamicMethodCalls (_arg0, _arg1, _arg2);
			
			base.InvokePersistent (_arg0, _arg1, _arg2);
		}
		
		protected Type GetActionType () { return typeof (QuickAction<T, U, V>); }
		/// <summary>Add a non persistent listener to the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void AddListener (QuickAction<T, U, V> _listener) { DynamicMethodCalls += _listener; }
		/// <summary>Remove a non persistent listener from the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void RemoveListener (QuickAction<T, U, V> _listener) { DynamicMethodCalls -= _listener; }
		/// <summary>Remove all non persistent listeners from the event.</summary>
		public void RemoveAllListeners () { DynamicMethodCalls = null; }
	}
	
	[Serializable]
	public class AdvancedEvent<T, U, V, W> : AdvancedEventBase
	{
		protected QuickAction<T, U, V, W> DynamicMethodCalls;
		
		/// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
		public void Invoke (T _arg0, U _arg1, V _arg2, W _arg3)
		{
			if (DynamicMethodCalls != null)
				DynamicMethodCalls (_arg0, _arg1, _arg2, _arg3);
			
			base.InvokePersistent (_arg0, _arg1, _arg2, _arg3);
		}
		
		protected Type GetActionType () { return typeof (QuickAction<T, U, V, W>); }
		/// <summary>Add a non persistent listener to the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void AddListener (QuickAction<T, U, V, W> _listener) { DynamicMethodCalls += _listener; }
		/// <summary>Remove a non persistent listener from the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void RemoveListener (QuickAction<T, U, V, W> _listener) { DynamicMethodCalls -= _listener; }
		/// <summary>Remove all non persistent listeners from the event.</summary>
		public void RemoveAllListeners () { DynamicMethodCalls = null; }
	}
	
	[Serializable] public class AdvancedObjectEvent : AdvancedEvent<Object> {}
	[Serializable] public class AdvancedStringEvent : AdvancedEvent<string> {}
	[Serializable] public class AdvancedIntEvent : AdvancedEvent<int> {}
	[Serializable] public class AdvancedFloatEvent : AdvancedEvent<float> {}
	[Serializable] public class AdvancedBoolEvent : AdvancedEvent<bool> {}
	[Serializable] public class AdvancedColorEvent : AdvancedEvent<Color> {}
	[Serializable] public class AdvancedVector2Event : AdvancedEvent<Vector2> {}
	[Serializable] public class AdvancedVector3Event : AdvancedEvent<Vector3> {}
}
