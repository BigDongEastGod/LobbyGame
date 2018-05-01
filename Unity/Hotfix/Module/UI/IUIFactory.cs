using UnityEngine;

namespace ETHotfix
{
	public interface IUIFactory
	{
		UI Create(Scene scene, string type, GameObject parent);
		
		UI Create(Scene scene, string type, GameObject parent, params object[] args);
		
		void Remove(string type);
	}
}