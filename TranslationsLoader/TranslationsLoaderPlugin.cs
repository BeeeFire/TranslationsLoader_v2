﻿using System.Collections.Generic;
using System.Linq;
using Il2CppInspector.PluginAPI.V100;
using Il2CppInspector.Reflection;

namespace TranslationsLoader
{
    public class TranslationsLoaderPlugin : IPlugin, ILoadPipeline
    {
        public string Id => "translations-loader";

        public string Name => "TranslationsLoader";

        public string Author => "OsOmE1";

        public string Version => "1.0.0";

        public string Description => "Performs deobfuscation with a nameTranslation.txt file";

        private PluginOptionFilePath TranslationsFileOption = new PluginOptionFilePath
        {
            Name = "translations",
            Description = "Path to nameTranslations.txt",
            MustExist = true,
            MustNotExist = false,
            IsFolder = false,
            Required = true
        };

        public List<IPluginOption> Options => new List<IPluginOption> { TranslationsFileOption };

        public void PostProcessTypeModel(TypeModel model, PluginPostProcessTypeModelEventInfo info)
        {
            Translations translations = new Translations(TranslationsFileOption.Value);
            IEnumerable<TypeInfo> types = model.Types;

            foreach(TypeInfo type in types)
            {
                DeobfuscateType(type, translations);
            }

            info.IsDataModified = true;
        }

        private void DeobfuscateType(TypeInfo type, Translations translations)
        {
            if (translations._translations.TryGetValue(type.CSharpName, out string typeTranslation))
            {
                //type.Name += "___" + typeTranslation;
                type.Name = type.CSharpName + "___" + typeTranslation;
            }
            
            foreach (FieldInfo field in type.DeclaredFields)
            {
                if (translations._translations.TryGetValue(field.CSharpName, out string fieldTranslation))
                {
                    //field.Name += "___" + fieldTranslation;
                    field.Name = field.CSharpName + "___" + fieldTranslation;
                }
            }

            foreach (PropertyInfo property in type.DeclaredProperties)
            {
                if (translations._translations.TryGetValue(property.CSharpName, out string propertyTranslation))
                {
                    //property.Name += "___" + propertyTranslation;
                    property.Name = property.CSharpName + "___" + propertyTranslation;
                }
            }

            foreach (MethodInfo method in type.DeclaredMethods)
            {
                if (translations._translations.TryGetValue(method.CSharpName, out string methodTranslation))
                {
                    //method.Name += "___" + methodTranslation;
                    method.Name = method.CSharpName + "___" + methodTranslation;

                }
            }

            foreach (TypeInfo nestedType in type.DeclaredNestedTypes)
            {
                DeobfuscateType(nestedType, translations);
            }
        }
    }
}
