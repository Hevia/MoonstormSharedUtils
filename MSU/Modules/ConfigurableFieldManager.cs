﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using BepInEx.Configuration;
using HG;
using System.Reflection;
using UnityEngine;

namespace Moonstorm
{
    public static class ConfigurableFieldManager
    {
        private static List<(Assembly, ConfigFile)> subscribedAssemblies = new List<(Assembly, ConfigFile)>();

        private static List<(List<Type>, ConfigFile)> typesToConfigure = new List<(List<Type>, ConfigFile)>();

        [SystemInitializer()]
        private static void Init()
        {
            RoR2Application.onLoad += ConfigureTypes;
        }

        public static void AddMod(Assembly assembly, ConfigFile configFile)
        {
            List<Type> types = new List<Type>();

            foreach(Type type in assembly.GetTypes())
            {
                try
                {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                                     .Where(field => field.GetCustomAttribute<ConfigurableField>() != null)
                                     .ToList();

                    if(fields.Count > 0)
                    {
                        types.Add(type);
                    }
                }
                catch (Exception e)
                {
                    MSULog.LogE($"An Exception has Ocurred. {e}");
                }
            }

            if(types.Count > 0)
            {
                (List<Type>, ConfigFile) tuple = (types, configFile);

                if(!typesToConfigure.Contains(tuple))
                {
                    typesToConfigure.Add(tuple);
                }
            }
        }

        private static void ConfigureTypes()
        {
            List<Type> count = new List<Type>();
            typesToConfigure.ForEach(type => count.AddRange(type.Item1));
            MSULog.LogI($"Configuring a total of {count.Count} Types.");

            foreach(var (types, config) in typesToConfigure)
            {
                foreach(Type type in types)
                {
                    try
                    {
                        ConfigureSelectedType(type, config);
                    }
                    catch (Exception e)
                    {
                        MSULog.LogE($"An Exception has Ocurred: {e}");
                    }
                }
            }
        }

        private static void ConfigureSelectedType(Type type, ConfigFile config)
        {
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public).Where(field => field.GetCustomAttribute<ConfigurableField>() != null);
            MSULog.LogD($"Configuring {fields.Count()} fields inside {type}");
            foreach (FieldInfo field in fields)
            {
                var configAttribute = field.GetCustomAttribute<ConfigurableField>(true);

                switch (field.GetValue(null))
                {
                    case String _text: Bind<String>(field, type, config, _text, configAttribute); break;
                    case Boolean _bool: Bind<Boolean>(field, type, config, _bool, configAttribute); break;
                    case Byte _byte: Bind<Byte>(field, type, config, _byte, configAttribute); break;
                    case SByte _sbyte: Bind<SByte>(field, type, config, _sbyte, configAttribute); break;
                    case Int16 _int16: Bind<Int16>(field, type, config, _int16, configAttribute); break;
                    case UInt16 _uint16: Bind<UInt16>(field, type, config, _uint16, configAttribute); break;
                    case Int32 _int32: Bind<Int32>(field, type, config, _int32, configAttribute); break;
                    case UInt32 _uint32: Bind<UInt32>(field, type, config, _uint32, configAttribute); break;
                    case Int64 _int64: Bind<Int64>(field, type, config, _int64, configAttribute); break;
                    case UInt64 _uint64: Bind<UInt64>(field, type, config, _uint64, configAttribute); break;
                    case Single _single: Bind<Single>(field, type, config, _single, configAttribute); break;
                    case Double _double: Bind<Double>(field, type, config, _double, configAttribute); break;
                    case Decimal _decimal: Bind<Decimal>(field, type, config, _decimal, configAttribute); break;
                    case Enum _enum: Bind<Enum>(field, type, config, _enum, configAttribute); break;
                    case Color _color: Bind<Color>(field, type, config, _color, configAttribute); break;
                    case Vector2 _vector2: Bind<Vector2>(field, type, config, _vector2, configAttribute); break;
                    case Vector3 _vector3: Bind<Vector3>(field, type, config, _vector3, configAttribute); break;
                    case Vector4 _vector4: Bind<Vector4>(field, type, config, _vector4, configAttribute); break;
                    case Quaternion _quaternion: Bind<Quaternion>(field, type, config, _quaternion, configAttribute); break;
                }
            }
        }

        private static void Bind<T>(FieldInfo field, Type type, ConfigFile config, T value, ConfigurableField configAttribute)
        {
            field.SetValue(type, config.Bind<T>(configAttribute.GetSection(type), configAttribute.GetName(field), value, configAttribute.GetDescription()).Value);
        }
    }
}