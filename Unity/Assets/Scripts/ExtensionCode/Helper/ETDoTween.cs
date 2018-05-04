using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

namespace ETModel
{
    public static class ETDoTweenEx
    {
        public static Tweener DOMove(this Transform target, Vector3 endValue, float duration, bool snapping = false)
        {
            return DOTween.To((DOGetter<Vector3>) (() => target.position), (DOSetter<Vector3>) (x => target.position = x), endValue, duration).SetOptions(snapping).SetTarget<Tweener>((object) target);
        }
    }
}