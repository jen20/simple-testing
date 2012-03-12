using JetBrains.Metadata.Reader.API;
using Simple.Testing.Framework;

namespace Simple.Testing.Resharper.Helpers
{
    public static class MetadataTypeInfoExtensions
    {
         public static bool ContainsSpecifications(this IMetadataTypeInfo metadataTypeInfo)
         {
             if (!(metadataTypeInfo.IsPublic || metadataTypeInfo.IsNestedPublic))
                 return false;

             var fields = metadataTypeInfo.GetFields();

             foreach (var field in fields)
             {
                 var fieldType = field.Type as IMetadataClassType;
                 if (fieldType == null)
                     continue;

                 if (typeof(Specification).IsAssignableFrom(fieldType.Type))
                     return true;
             }

             return false;
         }
    }
}