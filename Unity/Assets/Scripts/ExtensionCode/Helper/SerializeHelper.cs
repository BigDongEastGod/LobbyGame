using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ETModel
{
    public class SerializeHelper
    {
        private static SerializeHelper _instance;

        public static SerializeHelper Instance => _instance ?? (_instance = new SerializeHelper());
        
        public byte[] SerializeObject(object obj)  
        {  
            if (obj == null)  return null;  
            
            //内存实例  
            
            var ms = new MemoryStream(); 
            
            //创建序列化的实例  
            
            var formatter = new BinaryFormatter();  
            
            formatter.Serialize(ms, obj);//序列化对象，写入ms流中  
            
            var bytes = ms.GetBuffer();  
            
            return bytes;  
        }  
        public T DeserializeObject<T>(byte[] bytes)  
        {  
            object obj = null;

            if (bytes == null) return default(T); 
            
            //利用传来的byte[]创建一个内存流  

            var ms = new MemoryStream(bytes) {Position = 0};

            var formatter = new BinaryFormatter(); 
            
            //把内存流反序列成对象 
            
            obj = formatter.Deserialize(ms);  
            
            ms.Close();  
            
            return (T)obj;  
        }  
    }
}