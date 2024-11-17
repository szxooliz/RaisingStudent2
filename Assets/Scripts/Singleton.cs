using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Client
{
    public abstract class Singleton<T> where T : class
    {
        public static T Instance
        {
            get
            {
                return SingletonAllocator._Instance;
            }
        }

        public virtual void Init() { }

        protected Singleton() { }

        internal static class SingletonAllocator
        {
            internal static T _Instance = null; // Singleton 객체

            static SingletonAllocator()
            {
                CreateInstance(typeof(T));
                Initialize(typeof(T));
            }

            /// <summary>
            /// _Instance 생성 
            /// </summary>
            /// <param name="type"> 생성 타입 </param>
            static void CreateInstance(Type type)
            {
                ConstructorInfo constructorNonPublic = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]);

                ConstructorInfo[] constructorPublic = type.GetConstructors(
                    BindingFlags.Instance | BindingFlags.Public);

                if (constructorPublic != null && constructorPublic.Length > 0)
                {
                    Debug.LogError($"{type.FullName}의 생성자를 확인해주세요. Singleton 은 Public 생성자를 허용하지 않습니다. ");
                    return;
                }

                // 생성자에서 인스턴스를 생성
                if (constructorNonPublic != null)
                {
                    _Instance = (T)constructorNonPublic.Invoke(null);
                }
                else
                {
                    Debug.LogError("Singleton 객체를 생성할 수 없습니다. 비공개 생성자가 필요합니다.");
                }
            }

            // Singleton 오브젝트 공통 로직
            static void Initialize(Type type)
            {

            }

        }
    }

}
