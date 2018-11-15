//version: 2018-11-15-00

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jerry
{
    public class JerryUtil
    {
        #region 设置Layer

        /// <summary>
        /// SetLayer
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layerName"></param>
        public static void SetLayer(GameObject go, string layerName)
        {
            if (null == go)
            {
                return;
            }
            go.layer = LayerMask.NameToLayer(layerName);
        }

        /// <summary>
        /// SetLayerRecursively
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layerName"></param>
        public static void SetLayerRecursively(GameObject go, string layerName)
        {
            SetLayerRecursively(go, LayerMask.NameToLayer(layerName));
        }

        /// <summary>
        /// SetLayerRecursively
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        public static void SetLayerRecursively(GameObject go, int layer)
        {
            if (null == go)
            {
                return;
            }
            go.layer = layer;
            foreach (Transform child in go.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        #endregion 设置Layer

        #region 克隆对象

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GameObject CloneGo(CloneGoData data)
        {
            if (data == null)
            {
                return null;
            }

            if (data.parant != null && data.clean)
            {
                DestroyAllChildren(data.parant);
            }

            GameObject go = null;
            if (data.prefab == null)
            {
                go = new GameObject();
            }
            else
            {
                go = GameObject.Instantiate(data.prefab) as GameObject;
            }

            if (go.activeSelf != data.active)
            {
                go.SetActive(data.active);
            }
            if (string.IsNullOrEmpty(data.name) == false)
            {
                go.name = data.name;
            }
            if (data.parant != null)
            {
                go.transform.SetParent(data.parant);
            }
            go.transform.localScale = Vector3.one * data.scale;

            if (data.useOrignal && data.prefab != null)
            {
                go.transform.localPosition = data.prefab.transform.localPosition;
                go.transform.localEulerAngles = data.prefab.transform.localEulerAngles;
            }
            else
            {
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
            }

            if (data.isStretchUI
                && go.transform is RectTransform)
            {
                (go.transform as RectTransform).offsetMin = Vector2.zero;
                (go.transform as RectTransform).offsetMax = Vector2.zero;
            }
            return go;
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T CloneGo<T>(CloneGoData data) where T : MonoBehaviour
        {
            GameObject go = CloneGo(data);
            if (go == null)
            {
                return null;
            }
            return go.AddComponent<T>();
        }

        public class CloneGoData
        {
            /// <summary>
            /// 预设
            /// </summary>
            public GameObject prefab = null;
            /// <summary>
            /// 父节点，空则在外部
            /// </summary>
            public Transform parant = null;
            /// <summary>
            /// 名称，空则用默认
            /// </summary>
            public string name = null;
            /// <summary>
            /// 是否是要Stretch的UI
            /// </summary>
            public bool isStretchUI = false;
            /// <summary>
            /// 缩放系数
            /// </summary>
            public float scale = 1f;
            /// <summary>
            /// 清理父节点
            /// </summary>
            public bool clean = false;
            /// <summary>
            /// 使用原始位置信息
            /// </summary>
            public bool useOrignal = false;
            /// <summary>
            /// 激活
            /// </summary>
            public bool active = false;
        }

        #endregion 克隆对象

        #region 查找

        /// <summary>
        /// 查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static GameObject FindGo<T>(GameObject parent, string name, bool includeInactive = true) where T : Component
        {
            if (parent == null)
            {
                return null;
            }

            T t = FindCo<T>(parent.transform, name, includeInactive);

            return (t == null) ? null : t.gameObject;
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static GameObject FindGo(GameObject parent, string name, bool includeInactive = true)
        {
            if (parent == null)
            {
                return null;
            }

            Transform t = FindCo<Transform>(parent.transform, name, includeInactive);

            return (t == null) ? null : t.gameObject;
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static T FindCo<T>(GameObject parent, string name, bool includeInactive = true) where T : Component
        {
            if (parent == null)
            {
                return null;
            }

            return FindCo<T>(parent.transform, name, includeInactive);
        }

        /// <summary>
        /// 查找子结点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static T FindCo<T>(Transform parent, string name, bool includeInactive = true) where T : Component
        {
            if (parent == null)
            {
                return null;
            }

            foreach (T t in parent.GetComponentsInChildren<T>(includeInactive))
            {
                if (t.name.Equals(name))
                {
                    return t;
                }
            }

            return null;
        }

        #endregion 查找

        #region 删除子节点

        /// <summary>
        /// 删除所有儿子结点
        /// </summary>
        /// <param name="go"></param>
        /// <param name="immediate">是否立即删除，编辑器有时需要</param>
        public static void DestroyAllChildren(GameObject go, bool immediate = false)
        {
            if (go == null)
            {
                return;
            }

            List<GameObject> list = new List<GameObject>();

            for (int i = 0, imax = go.transform.childCount; i < imax; i++)
            {
                list.Add(go.transform.GetChild(i).gameObject);
            }

            while (list.Count > 0)
            {
                if (immediate)
                {
                    UnityEngine.Object.DestroyImmediate(list[0]);
                }
                else
                {
                    UnityEngine.Object.Destroy(list[0]);
                }
                list[0] = null;
                list.RemoveAt(0);
            }
            list.Clear();
        }

        /// <summary>
        /// 删除所有儿子结点
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="immediate">是否立即删除，编辑器有时需要</param>
        public static void DestroyAllChildren(Component comp, bool immediate = false)
        {
            if (comp == null)
            {
                return;
            }
            DestroyAllChildren(comp.gameObject, immediate);
        }

        #endregion 删除子节点

        #region 屏幕位置

        /// <summary>
        /// <para>获得鼠标位置</para>
        /// <para>移动设备用第一个触摸点</para>
        /// <para>返回值z轴为0</para>
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetMousePos()
        {
            Vector3 pos = Input.mousePosition;
            if (Application.platform == RuntimePlatform.Android
                || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    pos = Input.touches[0].position;
                }
            }
            pos.z = 0;
            return pos;
        }

        /// <summary>
        /// <para>当前鼠标位置(移动设备第一个触摸位置)点在EventSystem对象上面的信息</para>
        /// </summary>
        /// <returns></returns>
        public static List<RaycastResult> PointerOverEventSystemGameObjectInfo()
        {
            return PointerOverEventSystemGameObjectInfo(GetMousePos());
        }

        /// <summary>
        /// <para>指定屏幕位置点在EventSystem对象上面的信息</para>
        /// </summary>
        /// <returns></returns>
        public static List<RaycastResult> PointerOverEventSystemGameObjectInfo(Vector3 screenPoint)
        {
            PointerEventData ed = new PointerEventData(EventSystem.current);
            ed.position = screenPoint;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ed, results);
            return results;
        }

        /// <summary>
        /// <para>当前鼠标位置(移动设备第一个触摸位置)点是否在EventSystem对象上面</para>
        /// <para>可以指定Raycaster名称</para>
        /// </summary>
        /// <param name="raycasterNames"></param>
        /// <returns></returns>
        public static bool IsPointerOverEventSystemGameObject(params string[] raycasterNames)
        {
            if (raycasterNames != null && raycasterNames.Length > 0)
            {
                return IsPointerOverEventSystemGameObject(GetMousePos(), raycasterNames);
            }
            else
            {
                return EventSystem_IsPointerOverGameObject();
            }
        }

        /// <summary>
        /// <para>指定屏幕位置点是否在EventSystem对象上面</para>
        /// <para>可以指定Raycaster名称</para>
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <param name="raycasterNames"></param>
        /// <returns></returns>
        public static bool IsPointerOverEventSystemGameObject(Vector3 screenPoint, params string[] raycasterNames)
        {
            PointerEventData ed = new PointerEventData(EventSystem.current);
            ed.position = screenPoint;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ed, results);
            if (raycasterNames != null && raycasterNames.Length > 0)
            {
                List<string> raycasterNamesList = new List<string>(raycasterNames);
                foreach (RaycastResult r in results)
                {
                    if (raycasterNamesList.Contains(r.module.name))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return results.Count > 0;
            }
        }

        /// <summary>
        /// <para>当前鼠标位置(移动设备第一个触摸位置)点是否在EventSystem对象上面</para>
        /// <para>EventSystem的IsPointerOverGameObject重新封装</para>
        /// </summary>
        /// <returns></returns>
        private static bool EventSystem_IsPointerOverGameObject()
        {
            if (Application.platform == RuntimePlatform.Android
                || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
                }
                return false;
            }
            else
            {
                return EventSystem.current.IsPointerOverGameObject();
            }
        }

        #endregion 屏幕位置

        #region 坐标转化

        /// <summary>
        /// <para>计算UI节点相对Canvas的偏移量</para>
        /// <para>返回值z轴为0</para> 
        /// </summary>
        /// <param name="child">计算的UI节点</param>
        /// <param name="includeSelf">是否包括child自身的偏移</param>
        /// <param name="checkCanvasName">检查Cavans的名字，子节点如果加了额外的Canvas，可以通过这个参数过滤找到最外层的Canvas</param>
        /// <returns></returns>
        public static Vector3 CalUIPosRelateToCanvas(Transform child, bool includeSelf = false, string checkCanvasName = "")
        {
            Vector2 ret = Vector2.zero;
            if (child == null)
            {
                return ret;
            }
            if (includeSelf)
            {
                ret.x += child.localPosition.x;
                ret.y += child.localPosition.y;
            }
            while (child.parent != null)
            {
                child = child.parent;
                if (child.GetComponent<Canvas>() != null)
                {
                    if (string.IsNullOrEmpty(checkCanvasName)
                        || checkCanvasName.Equals(child.name))
                    {
                        break;
                    }
                }
                ret.x += child.localPosition.x;
                ret.y += child.localPosition.y;
            }
            return ret;
        }

        /// <summary>
        /// <para>World->Canvas</para>
        /// <para>返回值z轴为0</para>
        /// </summary>
        /// <param name="pos">世界位置</param>
        /// <param name="canvas">UI的Canvas</param>
        /// <param name="tf">使用结果的UI结点，空则是相对Canvas</param>
        /// <param name="checkCanvasName">检查Cavans的名字，子节点如果加了额外的Canvas，可以通过这个参数过滤找到最外层的Canvas</param>
        /// <returns></returns>
        public static Vector3 PosWorld2Canvas(Vector3 pos, Canvas canvas, Transform tf = null, string checkCanvasName = "")
        {
            Vector3 ret = Vector3.zero;
            if (canvas == null)
            {
                return ret;
            }

            ret = Camera.main.WorldToScreenPoint(pos);
            ret = PosScreen2Canvas(canvas, ret, tf, checkCanvasName);

            return ret;
        }

        /// <summary>
        /// <para>Canvas->Screen</para>
        /// <para>返回值z轴为0</para>
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="tf"></param>
        /// <param name="checkCanvasName">检查Cavans的名字，子节点如果加了额外的Canvas，可以通过这个参数过滤找到最外层的Canvas</param>
        /// <returns></returns>
        public static Vector3 PosCanvas2Screen(Canvas canvas, Transform tf, string checkCanvasName = "")
        {
            if (canvas == null ||
                tf == null)
            {
                return Vector2.zero;
            }

            Vector2 pos = CalUIPosRelateToCanvas(tf, true, checkCanvasName);
            return PosCanvas2Screen(canvas, pos);
        }

        /// <summary>
        /// <para>Canvas->Screen</para>
        /// <para>返回值z轴为0</para>
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="pos">点</param>
        /// <returns></returns>
        public static Vector3 PosCanvas2Screen(Canvas canvas, Vector3 pos)
        {
            Vector2 ret = Vector2.zero;
            if (canvas == null)
            {
                return ret;
            }

            Vector2 pos2 = pos;

            RectTransform canvasRect = canvas.transform as RectTransform;
            pos2 += canvasRect.sizeDelta * 0.5f;
            pos2 = new Vector2(pos2.x / canvasRect.sizeDelta.x, pos2.y / canvasRect.sizeDelta.y);
            ret = new Vector2(Screen.width * pos2.x, Screen.height * pos2.y);
            return ret;
        }

        /// <summary>
        /// <para>ScreenMouse->Canvas</para>
        /// <para>返回值z轴为0</para>
        /// </summary>
        /// <param name="canvas">Canvas</param>
        /// <param name="tf">使用结果的UI结点，空则是相对Canvas</param>
        /// <param name="checkCanvasName">检查Cavans的名字，子节点如果加了额外的Canvas，可以通过这个参数过滤找到最外层的Canvas</param>
        /// <returns></returns>
        public static Vector3 PosMouse2Canvas(Canvas canvas, Transform tf = null, string checkCanvasName = "")
        {
            return PosScreen2Canvas(canvas, GetMousePos(), tf, checkCanvasName);
        }

        /// <summary>
        /// <para>Screen->Canvas</para>
        /// <para>返回值z轴为0</para>
        /// </summary>
        /// <param name="canvas">Canvas</param>
        /// <param name="pos">Screen Position</param>
        /// <param name="tf">使用结果的UI结点，空则是相对Canvas</param>
        /// <param name="checkCanvasName">检查Cavans的名字，子节点如果加了额外的Canvas，可以通过这个参数过滤找到最外层的Canvas</param>
        /// <returns></returns>
        public static Vector3 PosScreen2Canvas(Canvas canvas, Vector3 pos, Transform tf = null, string checkCanvasName = "")
        {
            RectTransform canvasRect = canvas.transform as RectTransform;
            Vector2 viewportPos = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
            Vector3 ret = new Vector2(viewportPos.x * canvasRect.sizeDelta.x, viewportPos.y * canvasRect.sizeDelta.y) - canvasRect.sizeDelta * 0.5f;

            Vector3 relate = Vector3.zero;
            if (tf != null)
            {
                relate = CalUIPosRelateToCanvas(tf, false, checkCanvasName);
            }

            ret = ret - relate;

            return ret;
        }

        #endregion 坐标转化

        #region 时间转化

        public static double DateTime2Timestamp(System.DateTime t)
        {
            return t.Subtract(new System.DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        public static System.DateTime Timestamp2DateTime(double timestamp)
        {
            return (new System.DateTime(1970, 1, 1).ToLocalTime()).AddSeconds(timestamp);
        }

        #endregion 时间转化

        #region 数值转化

        /// <summary>
        /// <para>StringToTArray</para>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T[] String2TArray<T>(string str, char separator = ',')
        {
            List<T> list = new List<T>();

            if (string.IsNullOrEmpty(str))
            {
                return list.ToArray();
            }

            T tmp = default(T);

            string[] str_array = str.Split(separator);
            foreach (string s in str_array)
            {
                try
                {
                    tmp = (T)Convert.ChangeType(s, typeof(T));
                }
                catch (Exception ex)
                {
                    Debug.LogError(string.Format("StringToTArray error {0} : cant not change {1} to {2}", ex.Message, s, typeof(T)));
                    continue;
                }
                list.Add(tmp);
            }

            return list.ToArray();
        }

        /// <summary>
        /// TArrayToString
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string TArray2String<T>(T[] data, char separator = ',')
        {
            string strData = "";
            string sepStr = separator.ToString();
            bool first = true;
            foreach (T t in data)
            {
                strData += string.Format("{0}{1}", first ? "" : sepStr, t);
                first = false;
            }
            return strData;
        }

        #endregion 数值转化

        #region LayerMask处理

        /// <summary>
        /// <para>直接设置LayerMask为Everything或Nothing</para>
        /// <para>true:Everything</para>
        /// <para>false:Nothing</para>
        /// </summary>
        /// <param name="everythingOrNothing"></param>
        /// <returns></returns>
        public static int MakeLayerMask(bool everythingOrNothing)
        {
            if (everythingOrNothing)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 通过name构造LayerMask
        /// </summary>
        /// <param name="oldLayerMask">旧的layerMask</param>
        /// <param name="addNames">增加的</param>
        /// <param name="subNames">减去的</param>
        /// <returns></returns>
        public static int MakeLayerMask(int oldLayerMask, string[] addNames = null, string[] subNames = null)
        {
            int[] addIds = null;
            if (addNames != null)
            {
                addIds = new int[addNames.Length];
                int idx = 0;
                foreach (string name in addNames)
                {
                    addIds[idx++] = LayerMask.NameToLayer(name);
                }
            }
            int[] subIds = null;
            if (subNames != null)
            {
                subIds = new int[subNames.Length];
                int idx = 0;
                foreach (string name in subNames)
                {
                    subIds[idx++] = LayerMask.NameToLayer(name);
                }
            }
            return MakeLayerMask(oldLayerMask, addIds, subIds);
        }

        /// <summary>
        /// 通过id构造LayerMask
        /// </summary>
        /// <param name="oldLayerMask">旧的layerMask</param>
        /// <param name="addNames">增加的</param>
        /// <param name="subNames">减去的</param>
        /// <returns></returns>
        public static int MakeLayerMask(int oldLayerMask, int[] addIds = null, int[] subIds = null)
        {
            int ret = oldLayerMask;
            if (addIds != null)
            {
                foreach (int id in addIds)
                {
                    ret |= (1 << id);
                }
            }
            if (subIds != null)
            {
                foreach (int id in subIds)
                {
                    ret &= ~(1 << id);
                }
            }
            return ret;
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static bool LayerMaskContainAny(int mask, int[] check)
        {
            foreach (int id in check)
            {
                if ((mask & (1 << id)) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion LayerMask处理
        
        /// <summary>
        /// 获取Transform的Hieraichy路径
        /// </summary>
        /// <param name="tf"></param>
        /// <returns></returns>
        static public string GetTransformHieraichyPath(Transform tf)
        {
            if (tf == null)
            {
                return string.Empty;
            }
            string path = tf.name;
            while (tf.parent != null)
            {
                tf = tf.parent;
                if (string.IsNullOrEmpty(path))
                {
                    path = tf.name;
                }
                else
                {
                    path = tf.name + "/" + path;
                }
            }
            return path;
        }

        #region 颜色处理

        public static string ColorToHex(Color32 color)
        {
            string hex = string.Format("#{0:X2}{1:X2}{2:X2}", color.r, color.g, color.b, color.a);
            return hex;
        }

        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("0x", "");
            hex = hex.Replace("#", "");
            byte a = 255;
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        #endregion 颜色处理

        #region Shader处理

        /// <summary>
        /// 刷新Shader
        /// </summary>
        /// <param name="obj"></param>
        public static void RefreshShader(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }
            Renderer[] render = obj.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer re in render)
            {
                foreach (Material mat in re.sharedMaterials)
                {
                    if (mat != null)
                    {
                        RefreshShader(mat);
                    }
                }
            }
        }

        /// <summary>
        /// 刷新Shader
        /// </summary>
        /// <param name="mat"></param>
        public static void RefreshShader(Material mat)
        {
            if (mat != null && mat.shader != null)
            {
                mat.shader = Shader.Find(mat.shader.name);
            }
        }

        #endregion Shader处理
    }
}