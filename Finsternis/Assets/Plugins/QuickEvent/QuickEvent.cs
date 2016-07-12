using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace ByteSheep.Events
{
	public enum QuickSupportedTypes { Void = 0, String, Int, Float, Bool, Color, Vector2, Vector3, Object, GameObject, Transform }
	
	[Serializable]
	public class GenericMenuData
	{
		public int m_selectedComponent;
		public int m_selectedMember;
		public bool m_isDynamic;
	}
	
    [Serializable]
	public class QuickArgumentCache
	{
		public QuickSupportedTypes m_supportedType = QuickSupportedTypes.Void;
		public string m_stringArgument;
		public int m_intArgument;
		public float m_floatArgument;
		public bool m_boolArgument;
		public Color m_colorArgument;
		public Vector2 m_vector2Argument;
		public Vector3 m_vector3Argument;
		public Object m_objectArgument;
		public GameObject m_gameObjectArgument;
		public Transform m_transformArgument;
		
		public object GetArgumentValue ()
		{
			switch (m_supportedType)
			{
				case QuickSupportedTypes.String:
					return m_stringArgument;
				case QuickSupportedTypes.Int:
					return m_intArgument;
				case QuickSupportedTypes.Float:
					return m_floatArgument;
				case QuickSupportedTypes.Bool:
					return m_boolArgument;
				case QuickSupportedTypes.Color:
					return m_colorArgument;
				case QuickSupportedTypes.Vector2:
					return m_vector2Argument;
				case QuickSupportedTypes.Vector3:
					return m_vector3Argument;
				case QuickSupportedTypes.Object:
					return m_objectArgument;
				case QuickSupportedTypes.GameObject:
					return m_gameObjectArgument;
				case QuickSupportedTypes.Transform:
					return m_transformArgument;
				default:
					return null;
			}
		}
		
		public Type GetArgumentType ()
		{
			switch (m_supportedType)
			{
				case QuickSupportedTypes.String:
					return typeof (string);
				case QuickSupportedTypes.Int:
					return typeof (int);
				case QuickSupportedTypes.Float:
					return typeof (float);
				case QuickSupportedTypes.Bool:
					return typeof (bool);
				case QuickSupportedTypes.Color:
					return typeof (Color);
				case QuickSupportedTypes.Vector2:
					return typeof (Vector2);
				case QuickSupportedTypes.Vector3:
					return typeof (Vector3);
				case QuickSupportedTypes.Object:
					return typeof (Object);
				case QuickSupportedTypes.GameObject:
					return typeof (GameObject);
				case QuickSupportedTypes.Transform:
					return typeof (Transform);
				default:
					return null;
			}
		}
	}
	
	[Serializable]
	public class QuickActionGroup
	{
		public QuickAction QuickDelegate;
		public QuickAction<string> QuickStringDelegate;
		public QuickAction<int> QuickIntDelegate;
		public QuickAction<float> QuickFloatDelegate;
		public QuickAction<bool> QuickBoolDelegate;
		public QuickAction<Color> QuickColorDelegate;
		public QuickAction<Vector2> QuickVector2Delegate;
		public QuickAction<Vector3> QuickVector3Delegate;
		public QuickAction<Object> QuickObjectDelegate;
		public QuickAction<GameObject> QuickGameObjectDelegate;
		public QuickAction<Transform> QuickTransformDelegate;
		
		public void SetDelegate (Delegate _listener, QuickSupportedTypes _type)
		{
			switch (_type)
			{
				case QuickSupportedTypes.String:
				QuickStringDelegate = (QuickAction<string>) _listener;
				break;
				case QuickSupportedTypes.Int:
				QuickIntDelegate = (QuickAction<int>) _listener;
				break;
				case QuickSupportedTypes.Float:
				QuickFloatDelegate = (QuickAction<float>) _listener;
				break;
				case QuickSupportedTypes.Bool:
				QuickBoolDelegate = (QuickAction<bool>) _listener;
				break;
				case QuickSupportedTypes.Color:
				QuickColorDelegate = (QuickAction<Color>) _listener;
				break;
				case QuickSupportedTypes.Vector2:
				QuickVector2Delegate = (QuickAction<Vector2>) _listener;
				break;
				case QuickSupportedTypes.Vector3:
				QuickVector3Delegate = (QuickAction<Vector3>) _listener;
				break;
				case QuickSupportedTypes.Object:
				QuickObjectDelegate = (QuickAction<Object>) _listener;
				break;
				case QuickSupportedTypes.GameObject:
				QuickGameObjectDelegate = (QuickAction<GameObject>) _listener;
				break;
				case QuickSupportedTypes.Transform:
				QuickTransformDelegate = (QuickAction<Transform>) _listener;
				break;
				default:
				QuickDelegate = (QuickAction) _listener;
				break;
			}
		}
		
		public void Invoke (object _argument, QuickSupportedTypes _type)
		{
			switch (_type)
			{
				case QuickSupportedTypes.String:
				if (QuickStringDelegate != null)
					QuickStringDelegate (_argument as string);
				break;
				case QuickSupportedTypes.Int:
				if (QuickIntDelegate != null)
					QuickIntDelegate ((int) _argument);
				break;
				case QuickSupportedTypes.Float:
				if (QuickFloatDelegate != null)
					QuickFloatDelegate ((float) _argument);
				break;
				case QuickSupportedTypes.Bool:
				if (QuickBoolDelegate != null)
					QuickBoolDelegate ((bool) _argument);
				break;
				case QuickSupportedTypes.Color:
				if (QuickColorDelegate != null)
					QuickColorDelegate ((Color) _argument);
				break;
				case QuickSupportedTypes.Vector2:
				if (QuickVector2Delegate != null)
					QuickVector2Delegate ((Vector2) _argument);
				break;
				case QuickSupportedTypes.Vector3:
				if (QuickVector3Delegate != null)
					QuickVector3Delegate ((Vector3) _argument);
				break;
				case QuickSupportedTypes.Object:
				if (QuickObjectDelegate != null)
					QuickObjectDelegate (_argument as Object);
				break;
				case QuickSupportedTypes.GameObject:
				if (QuickGameObjectDelegate != null)
					QuickGameObjectDelegate (_argument as GameObject);
				break;
				case QuickSupportedTypes.Transform:
				if (QuickTransformDelegate != null)
					QuickTransformDelegate (_argument as Transform);
				break;
				default:
				if (QuickDelegate != null)
					QuickDelegate ();
				break;
			}
		}
	}
	
	[Serializable]
	public class QuickPersistentCallGroup
	{
		public List<QuickPersistentCall> m_calls = new List<QuickPersistentCall> ();
	}
	
	[Serializable]
	public class QuickPersistentCall
	{
		public GenericMenuData m_genericMenuData;
		public Object m_target;
		public string m_memberName;
		public MemberTypes m_memberType;
		public FieldInfo m_fieldInfo;
		public QuickArgumentCache m_argument;
		public object m_argumentValue;
		public bool m_isDynamic;
		public bool m_isCallEnabled;
		[HideInInspector] public QuickActionGroup m_actionGroup;
		
		public void Invoke ()
		{
			if (!m_isCallEnabled || m_target == null) return;
			
			if (m_memberType == MemberTypes.Field)
				m_fieldInfo.SetValue (m_target, m_argumentValue);
			else if (!m_isDynamic)
				m_actionGroup.Invoke (m_argumentValue, m_argument.m_supportedType);
		}
		
		public void SetDynamicArgument (object _dynamicArgument)
		{
			if (m_isCallEnabled && m_isDynamic)
				m_argumentValue = _dynamicArgument;
		}
	}
	
	public delegate void QuickAction ();
	public delegate void QuickAction<T> (T _arg0);
	public delegate void QuickAction<T, U> (T _arg0, U _arg1);
	public delegate void QuickAction<T, U, V> (T _arg0, U _arg1, V _arg2);
	public delegate void QuickAction<T, U, V, W> (T _arg0, U _arg1, V _arg2, W _arg3);
	
	[Serializable]
	public abstract class QuickEventBase : ISerializationCallbackReceiver
	{
		#pragma warning disable 0414
		[SerializeField] private float m_inspectorListHeight = 40f;
		#pragma warning restore 0414
		public QuickPersistentCallGroup m_persistentCalls;
		
		protected void InvokePersistent ()
		{
			for (int i = 0; i < m_persistentCalls.m_calls.Count; i++)
				m_persistentCalls.m_calls[i].Invoke ();
		}
		
		// Update the argument value of each dynamic call, with the argument passed to QuickEvent<T>.Invoke (T _arg0);
		protected void InvokePersistent (object _dynamicArgument)
		{
			for (int i = 0; i < m_persistentCalls.m_calls.Count; i++)
			{
				m_persistentCalls.m_calls[i].SetDynamicArgument (_dynamicArgument);
				m_persistentCalls.m_calls[i].Invoke ();
			}
		}
		
		protected Type GetActionType (QuickSupportedTypes _type)
		{
			switch (_type)
			{
				case QuickSupportedTypes.String:
				return typeof (QuickAction<string>);
				case QuickSupportedTypes.Int:
				return typeof (QuickAction<int>);
				case QuickSupportedTypes.Float:
				return typeof (QuickAction<float>);
				case QuickSupportedTypes.Bool:
				return typeof (QuickAction<bool>);
				case QuickSupportedTypes.Color:
				return typeof (QuickAction<Color>);
				case QuickSupportedTypes.Vector2:
				return typeof (QuickAction<Vector2>);
				case QuickSupportedTypes.Vector3:
				return typeof (QuickAction<Vector3>);
				case QuickSupportedTypes.Object:
				return typeof (QuickAction<Object>);
				case QuickSupportedTypes.GameObject:
				return typeof (QuickAction<GameObject>);
				case QuickSupportedTypes.Transform:
				return typeof (QuickAction<Transform>);
				default:
				return typeof (QuickAction);
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
				QuickPersistentCall persistentCall = m_persistentCalls.m_calls[i];
				
				if (!persistentCall.m_isCallEnabled || persistentCall.m_target == null || persistentCall.m_memberName == "" || persistentCall.m_argument == null || persistentCall.m_isDynamic)
					continue;
				
				Type argumentType = persistentCall.m_argument.GetArgumentType ();
				persistentCall.m_argumentValue = persistentCall.m_argument.GetArgumentValue ();
				
				if (persistentCall.m_memberType == MemberTypes.Method)
				{
					MethodInfo methodInfo = persistentCall.m_target.GetType ().GetMethod (persistentCall.m_memberName, (argumentType == null) ? new Type[] {} : new Type[] { argumentType });
					if (methodInfo != null)
						persistentCall.m_actionGroup.SetDelegate (Delegate.CreateDelegate (GetActionType (persistentCall.m_argument.m_supportedType), persistentCall.m_target, methodInfo, true), persistentCall.m_argument.m_supportedType);
				}
				else if (persistentCall.m_memberType == MemberTypes.Property)
				{
					PropertyInfo propertyInfo = persistentCall.m_target.GetType ().GetProperty (persistentCall.m_memberName);
					if (propertyInfo != null)
					{
						MethodInfo setMethodInfo = propertyInfo.GetSetMethod ();
						if (setMethodInfo != null)
							persistentCall.m_actionGroup.SetDelegate (Delegate.CreateDelegate (GetActionType (persistentCall.m_argument.m_supportedType), persistentCall.m_target, setMethodInfo, true), persistentCall.m_argument.m_supportedType);
					}
				}
				else if (persistentCall.m_memberType == MemberTypes.Field)
				{
					persistentCall.m_fieldInfo = persistentCall.m_target.GetType ().GetField (persistentCall.m_memberName);
				}
			}
		}
	}
	
	[Serializable]
	public class QuickEvent : QuickEventBase
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
	public class QuickEvent<T> : QuickEventBase
	{
		protected QuickAction<T> DynamicMethodCalls;
		protected QuickAction<T> PersistentDynamicMethodCalls;
		
		/// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
		public void Invoke (T _arg0)
		{
			if (DynamicMethodCalls != null)
				DynamicMethodCalls (_arg0);
			
			if (PersistentDynamicMethodCalls != null)
				PersistentDynamicMethodCalls (_arg0);
			
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
		
		protected void AddPersistentListener (QuickAction<T> _listener) { PersistentDynamicMethodCalls += _listener; }
		protected void RemovePersistentListener (QuickAction<T> _listener) { PersistentDynamicMethodCalls -= _listener; }
		public void RemoveAllPersistentListeners () { PersistentDynamicMethodCalls = null; }
		
		public override void OnAfterDeserialize ()
		{
			RemoveAllPersistentListeners ();
			base.OnAfterDeserialize ();
			
			for (int i = 0; i < m_persistentCalls.m_calls.Count; i++)
			{
				QuickPersistentCall persistentCall = m_persistentCalls.m_calls[i];
				
				if (!persistentCall.m_isCallEnabled || persistentCall.m_target == null || persistentCall.m_memberName == "" || !persistentCall.m_isDynamic)
					continue;
				
				if (persistentCall.m_memberType == MemberTypes.Method)
				{
					MethodInfo methodInfo = persistentCall.m_target.GetType ().GetMethod (persistentCall.m_memberName, new Type[] { typeof (T) });
					if (methodInfo != null)
						AddPersistentListener ((QuickAction<T>) Delegate.CreateDelegate (GetActionType (), persistentCall.m_target, methodInfo, false));
				}
				else if (persistentCall.m_memberType == MemberTypes.Property)
				{
					PropertyInfo propertyInfo = persistentCall.m_target.GetType ().GetProperty (persistentCall.m_memberName);
					if (propertyInfo != null)
					{
						MethodInfo setMethodInfo = propertyInfo.GetSetMethod ();
						if (setMethodInfo != null)
							AddPersistentListener ((QuickAction<T>) Delegate.CreateDelegate (GetActionType (), persistentCall.m_target, setMethodInfo, false));
					}
				}
				else if (persistentCall.m_memberType == MemberTypes.Field)
				{
					persistentCall.m_fieldInfo = persistentCall.m_target.GetType ().GetField (persistentCall.m_memberName);
				}
			}
		}
	}
	
	[Serializable]
	public class QuickEvent<T, U> : QuickEventBase
	{
		protected QuickAction<T, U> DynamicMethodCalls;
		protected QuickAction<T, U> PersistentDynamicMethodCalls;
		
		/// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
		public void Invoke (T _arg0, U _arg1)
		{
			if (DynamicMethodCalls != null)
				DynamicMethodCalls (_arg0, _arg1);
			
			if (PersistentDynamicMethodCalls != null)
				PersistentDynamicMethodCalls (_arg0, _arg1);
			
			base.InvokePersistent ();
		}
		
		/// <summary>Add a non persistent listener to the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void AddListener (QuickAction<T, U> _listener) { DynamicMethodCalls += _listener; }
		/// <summary>Remove a non persistent listener from the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void RemoveListener (QuickAction<T, U> _listener) { DynamicMethodCalls -= _listener; }
		/// <summary>Remove all non persistent listeners from the event.</summary>
		public void RemoveAllListeners () { DynamicMethodCalls = null; }
		
		protected void AddPersistentListener (QuickAction<T, U> _listener) { PersistentDynamicMethodCalls += _listener; }
		protected void RemovePersistentListener (QuickAction<T, U> _listener) { PersistentDynamicMethodCalls -= _listener; }
		public void RemoveAllPersistentListeners () { PersistentDynamicMethodCalls = null; }
		
		public override void OnAfterDeserialize ()
		{
			RemoveAllPersistentListeners ();
			base.OnAfterDeserialize ();
		}
	}
	
	[Serializable]
	public class QuickEvent<T, U, V> : QuickEventBase
	{
		protected QuickAction<T, U, V> DynamicMethodCalls;
		protected QuickAction<T, U, V> PersistentDynamicMethodCalls;
		
		/// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
		public void Invoke (T _arg0, U _arg1, V _arg2)
		{
			if (DynamicMethodCalls != null)
				DynamicMethodCalls (_arg0, _arg1, _arg2);
			
			if (PersistentDynamicMethodCalls != null)
				PersistentDynamicMethodCalls (_arg0, _arg1, _arg2);
			
			base.InvokePersistent ();
		}
		
		/// <summary>Add a non persistent listener to the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void AddListener (QuickAction<T, U, V> _listener) { DynamicMethodCalls += _listener; }
		/// <summary>Remove a non persistent listener from the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void RemoveListener (QuickAction<T, U, V> _listener) { DynamicMethodCalls -= _listener; }
		/// <summary>Remove all non persistent listeners from the event.</summary>
		public void RemoveAllListeners () { DynamicMethodCalls = null; }
		
		protected void AddPersistentListener (QuickAction<T, U, V> _listener) { PersistentDynamicMethodCalls += _listener; }
		protected void RemovePersistentListener (QuickAction<T, U, V> _listener) { PersistentDynamicMethodCalls -= _listener; }
		public void RemoveAllPersistentListeners () { PersistentDynamicMethodCalls = null; }
		
		public override void OnAfterDeserialize ()
		{
			RemoveAllPersistentListeners ();
			base.OnAfterDeserialize ();
		}
	}
	
	[Serializable]
	public class QuickEvent<T, U, V, W> : QuickEventBase
	{
		protected QuickAction<T, U, V, W> DynamicMethodCalls;
		protected QuickAction<T, U, V, W> PersistentDynamicMethodCalls;
		
		/// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
		public void Invoke (T _arg0, U _arg1, V _arg2, W _arg3)
		{
			if (DynamicMethodCalls != null)
				DynamicMethodCalls (_arg0, _arg1, _arg2, _arg3);
			
			if (PersistentDynamicMethodCalls != null)
				PersistentDynamicMethodCalls (_arg0, _arg1, _arg2, _arg3);
			
			base.InvokePersistent ();
		}
		
		/// <summary>Add a non persistent listener to the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void AddListener (QuickAction<T, U, V, W> _listener) { DynamicMethodCalls += _listener; }
		/// <summary>Remove a non persistent listener from the event.</summary>
		/// <param name="_listener">Callback function.</param>
		public void RemoveListener (QuickAction<T, U, V, W> _listener) { DynamicMethodCalls -= _listener; }
		/// <summary>Remove all non persistent listeners from the event.</summary>
		public void RemoveAllListeners () { DynamicMethodCalls = null; }
		
		protected void AddPersistentListener (QuickAction<T, U, V, W> _listener) { PersistentDynamicMethodCalls += _listener; }
		protected void RemovePersistentListener (QuickAction<T, U, V, W> _listener) { PersistentDynamicMethodCalls -= _listener; }
		public void RemoveAllPersistentListeners () { PersistentDynamicMethodCalls = null; }
		
		public override void OnAfterDeserialize ()
		{
			RemoveAllPersistentListeners ();
			base.OnAfterDeserialize ();
		}
	}
	
	[Serializable] public class QuickStringEvent : QuickEvent<string> {}
	[Serializable] public class QuickIntEvent : QuickEvent<int> {}
	[Serializable] public class QuickFloatEvent : QuickEvent<float> {}
	[Serializable] public class QuickBoolEvent : QuickEvent<bool> {}
	[Serializable] public class QuickColorEvent : QuickEvent<Color> {}
	[Serializable] public class QuickVector2Event : QuickEvent<Vector2> {}
	[Serializable] public class QuickVector3Event : QuickEvent<Vector3> {}
	[Serializable] public class QuickObjectEvent : QuickEvent<Object> {}
	[Serializable] public class QuickGameObjectEvent : QuickEvent<GameObject> {}
	[Serializable] public class QuickTransformEvent : QuickEvent<Transform> {}
}
