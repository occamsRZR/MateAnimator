using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

using Holoville.HOTween;
using Holoville.HOTween.Plugins;

[AddComponentMenu("")]
public class AMPropertyAction : AMAction {

    public int valueType;

    public Component component;
    public int endFrame;
    public string propertyName;
    public string fieldName;
    public string methodName;
    public string[] methodParameterTypes;
    private MethodInfo cachedMethodInfo;
    private PropertyInfo cachedPropertyInfo;
    private FieldInfo cachedFieldInfo;
    public PropertyInfo propertyInfo {
        get {
            if(cachedPropertyInfo != null) return cachedPropertyInfo;
            if(!component || propertyName == null) return null;
            cachedPropertyInfo = component.GetType().GetProperty(propertyName);
            return cachedPropertyInfo;
        }
        set {
            if(value != null) propertyName = value.Name;
            else propertyName = null;
            cachedPropertyInfo = value;

        }
    }
    public FieldInfo fieldInfo {
        get {
            if(cachedFieldInfo != null) return cachedFieldInfo;
            if(!component || fieldName == null) return null;
            cachedFieldInfo = component.GetType().GetField(fieldName);
            return cachedFieldInfo;
        }
        set {
            if(value != null) fieldName = value.Name;
            else fieldName = null;
            cachedFieldInfo = value;
        }
    }		// holds a field such as variables for user scripts, should be null if property is used
    public MethodInfo methodInfo {
        get {
            if(cachedMethodInfo != null) return cachedMethodInfo;
            if(!component || methodName == null) return null;
            Type[] t = new Type[methodParameterTypes.Length];
            for(int i = 0; i < methodParameterTypes.Length; i++) t[i] = Type.GetType(methodParameterTypes[i]);
            cachedMethodInfo = component.GetType().GetMethod(methodName, t);
            return cachedMethodInfo;
        }
        set {
            if(value != null) methodName = value.Name;
            else methodName = null;
            cachedMethodInfo = value;
        }
    }
    public double start_val;		// value as double (includes int/long)
    public Vector4 start_vect4;

    public Vector2 start_vect2 { get { return new Vector2(start_vect4.x, start_vect4.y); } set { start_vect4.Set(value.x, value.y, 0, 0); } }
    public Vector3 start_vect3 { get { return new Vector3(start_vect4.x, start_vect4.y, start_vect4.z); } set { start_vect4.Set(value.x, value.y, value.z, 0); } }
    public Color start_color { get { return new Color(start_vect4.x, start_vect4.y, start_vect4.z, start_vect4.w); } set { start_vect4.Set(value.r, value.g, value.b, value.a); } }
    public Rect start_rect { get { return new Rect(start_vect4.x, start_vect4.y, start_vect4.z, start_vect4.w); } set { start_vect4.Set(value.xMin, value.yMin, value.width, value.height); } }
    public Quaternion start_quat { get { return new Quaternion(start_vect4.x, start_vect4.y, start_vect4.z, start_vect4.w); } set { start_vect4.Set(value.x, value.y, value.z, value.w); } }

    public double end_val;			// value as double (includes int/long)
    public Vector4 end_vect4;

    public Vector2 end_vect2 { get { return new Vector2(end_vect4.x, end_vect4.y); } set { end_vect4.Set(value.x, value.y, 0, 0); } }
    public Vector3 end_vect3 { get { return new Vector3(end_vect4.x, end_vect4.y, end_vect4.z); } set { end_vect4.Set(value.x, value.y, value.z, 0); } }
    public Color end_color { get { return new Color(end_vect4.x, end_vect4.y, end_vect4.z, end_vect4.w); } set { end_vect4.Set(value.r, value.g, value.b, value.a); } }
    public Rect end_rect { get { return new Rect(end_vect4.x, end_vect4.y, end_vect4.z, end_vect4.w); } set { end_vect4.Set(value.xMin, value.yMin, value.width, value.height); } }
    public Quaternion end_quat { get { return new Quaternion(end_vect4.x, end_vect4.y, end_vect4.z, end_vect4.w); } set { end_vect4.Set(value.x, value.y, value.z, value.w); } }

    public override string ToString(int codeLanguage, int frameRate) {
        /*if (endFrame == -1 || targetsAreEqual()) return null;
        string memberInfoType = "";
        if(fieldInfo != null) memberInfoType = "fieldinfo";
        else if(propertyInfo != null) memberInfoType = "propertyinfo";
        else if(methodInfo != null) memberInfoType = "methodinfo";
        else {
            Debug.LogError("Animator: No FieldInfo or PropertyInfo set.");
            return "( Error: No FieldInfo, PropertyInfo or MethodInfo set. )";
        }
        if(codeLanguage == 0) {
            // c#
            if(valueType == (int)AMPropertyTrack.ValueType.MorphChannels) return "AMTween.PropertyTo(obj.gameObject, AMTween.Hash(\"delay\", "+getWaitTime(frameRate,0f)+"f, \"time\", "+getTime(frameRate)+"f, \""+memberInfoType+"\", obj.memberinfo, \"methodtype\", \"morph\", \"from\", "+getFloatArrayString(codeLanguage, start_morph)+", \"to\", "+getFloatArrayString(codeLanguage, end_morph)+", "+getEaseString(codeLanguage)+"));";
            if(AMPropertyTrack.isValueTypeNumeric(valueType)) return "AMTween.PropertyTo(obj.gameObject, AMTween.Hash(\"delay\", "+getWaitTime(frameRate,0f)+"f, \"time\", "+getTime(frameRate)+"f, \""+memberInfoType+"\", obj.memberinfo, \"from\", "+start_val+"f,\"to\", "+end_val+"f, "+getEaseString(codeLanguage)+"));";
            if(valueType == (int)AMPropertyTrack.ValueType.Vector2) return "AMTween.PropertyTo(obj.gameObject, AMTween.Hash(\"delay\", "+getWaitTime(frameRate,0f)+"f, \"time\", "+getTime(frameRate)+"f, \""+memberInfoType+"\", obj.memberinfo, \"from\", new Vector2("+start_vect2.x+"f, "+start_vect2.y+"f), \"to\", new Vector2("+end_vect2.x+"f, "+end_vect2.y+"f), "+getEaseString(codeLanguage)+"));";
            if(valueType == (int)AMPropertyTrack.ValueType.Vector3) return "AMTween.PropertyTo(obj.gameObject, AMTween.Hash(\"delay\", "+getWaitTime(frameRate,0f)+"f, \"time\", "+getTime(frameRate)+"f, \""+memberInfoType+"\", obj.memberinfo, \"from\", new Vector3("+start_vect3.x+"f, "+start_vect3.y+"f, "+start_vect3.z+"f), \"to\", new Vector3("+end_vect3.x+"f, "+end_vect3.y+"f, "+end_vect3.z+"f), "+getEaseString(codeLanguage)+"));";
            if(valueType == (int)AMPropertyTrack.ValueType.Color) return "AMTween.PropertyTo(obj.gameObject, AMTween.Hash(\"delay\", "+getWaitTime(frameRate,0f)+"f, \"time\", "+getTime(frameRate)+"f, \""+memberInfoType+"\", obj.memberinfo, \"from\", new Color("+start_color.r+"f, "+start_color.g+"f, "+start_color.b+"f, "+start_color.a+"f), \"to\", new Color("+end_color.r+"f, "+end_color.g+"f, "+end_color.b+"f, "+end_color.a+"f), "+getEaseString(codeLanguage)+"));";
            if(valueType == (int)AMPropertyTrack.ValueType.Rect) return "AMTween.PropertyTo(obj.gameObject, AMTween.Hash(\"delay\", "+getWaitTime(frameRate,0f)+"f, \"time\", "+getTime(frameRate)+"f, \""+memberInfoType+"\", obj.memberinfo, \"from\", new Rect("+start_rect.x+"f, "+start_rect.y+"f, "+start_rect.width+"f, "+start_rect.height+"f), \"to\", new Rect("+end_rect.x+"f, "+end_rect.y+"f, "+end_rect.width+"f, "+end_rect.height+"f), "+getEaseString(codeLanguage)+"));";
            return "( Error: ValueType "+valueType+" not found )";
        } else {
            // js
            if(valueType == (int)AMPropertyTrack.ValueType.MorphChannels) return "AMTween.PropertyTo(obj.gameObject, {\"delay\": "+getWaitTime(frameRate,0f)+", \"time\": "+getTime(frameRate)+", \""+memberInfoType+"\": obj.memberinfo, \"methodtype\": \"morph\", \"from\": "+getFloatArrayString(codeLanguage, start_morph)+", \"to\": "+getFloatArrayString(codeLanguage, end_morph)+", "+getEaseString(codeLanguage)+"));";
            if(AMPropertyTrack.isValueTypeNumeric(valueType)) return "AMTween.PropertyTo(obj.gameObject, {\"delay\": "+getWaitTime(frameRate,0f)+", \"time\": "+getTime(frameRate)+", \""+memberInfoType+"\": obj.memberinfo, \"from\": "+start_val+",\"to\": "+end_val+", "+getEaseString(codeLanguage)+"});";
            if(valueType == (int)AMPropertyTrack.ValueType.Vector2) return "AMTween.PropertyTo(obj.gameObject, {\"delay\": "+getWaitTime(frameRate,0f)+", \"time\": "+getTime(frameRate)+", \""+memberInfoType+"\": obj.memberinfo, \"from\": Vector2("+start_vect2.x+", "+start_vect2.y+"), \"to\": Vector2("+end_vect2.x+", "+end_vect2.y+"), "+getEaseString(codeLanguage)+"});";
            if(valueType == (int)AMPropertyTrack.ValueType.Vector3) return "AMTween.PropertyTo(obj.gameObject, {\"delay\": "+getWaitTime(frameRate,0f)+", \"time\": "+getTime(frameRate)+", \""+memberInfoType+"\": obj.memberinfo, \"from\": Vector3("+start_vect3.x+", "+start_vect3.y+", "+start_vect3.z+"), \"to\": Vector3("+end_vect3.x+", "+end_vect3.y+", "+end_vect3.z+"), "+getEaseString(codeLanguage)+"});";
            if(valueType == (int)AMPropertyTrack.ValueType.Color) return "AMTween.PropertyTo(obj.gameObject, {\"delay\": "+getWaitTime(frameRate,0f)+", \"time\": "+getTime(frameRate)+", \""+memberInfoType+"\": obj.memberinfo, \"from\": Color("+start_color.r+", "+start_color.g+", "+start_color.b+", "+start_color.a+"), \"to\": Color("+end_color.r+", "+end_color.g+", "+end_color.b+", "+end_color.a+"), "+getEaseString(codeLanguage)+"});";
            if(valueType == (int)AMPropertyTrack.ValueType.Rect) return "AMTween.PropertyTo(obj.gameObject, {\"delay\": "+getWaitTime(frameRate,0f)+", \"time\": "+getTime(frameRate)+", \""+memberInfoType+"\": obj.memberinfo, \"from\": Rect("+start_rect.x+", "+start_rect.y+", "+start_rect.width+", "+start_rect.height+"), \"to\": Rect("+end_rect.x+", "+end_rect.y+", "+end_rect.width+", "+end_rect.height+"), "+getEaseString(codeLanguage)+"});";
            return "( Error: ValueType "+valueType+" not found )";
        }*/
        Debug.LogError("need implement");
        return "";
    }

    public override int getNumberOfFrames() {
        return endFrame - startFrame;
    }
    public float getTime(int frameRate) {
        return (float)getNumberOfFrames() / (float)frameRate;
    }
    public override Tweener buildTweener(Sequence sequence, int frameRate) {
        if(targetsAreEqual()) return null;
        if((endFrame == -1) || !component || ((fieldInfo == null) && (propertyInfo == null) && (methodInfo == null))) return null;

        string varName = null;

        if(fieldInfo != null) {
            varName = fieldName;
        }
        else if(propertyInfo != null) {
            varName = propertyName;
        }
        //return HOTween.To(obj, getTime(frameRate), new TweenParms().Prop(isLocal ? "localRotation" : "rotation", new AMPlugQuaternionSlerp(endRotation)).Ease(easeCurve));
        if(varName != null) {
            if(hasCustomEase()) {
                switch((AMPropertyTrack.ValueType)valueType) {
                    case AMPropertyTrack.ValueType.Integer:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, Convert.ToInt32(end_val)).Ease(easeCurve));
                    case AMPropertyTrack.ValueType.Float:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, Convert.ToSingle(end_val)).Ease(easeCurve));
                    case AMPropertyTrack.ValueType.Double:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, new AMPlugDouble(end_val)).Ease(easeCurve));
                    case AMPropertyTrack.ValueType.Long:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, new AMPlugLong(Convert.ToInt64(end_val))).Ease(easeCurve));
                    case AMPropertyTrack.ValueType.Vector2:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, end_vect2).Ease(easeCurve));
                    case AMPropertyTrack.ValueType.Vector3:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, end_vect3).Ease(easeCurve));
                    case AMPropertyTrack.ValueType.Color:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, end_color).Ease(easeCurve));
                    case AMPropertyTrack.ValueType.Rect:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, end_rect).Ease(easeCurve));
                    case AMPropertyTrack.ValueType.Vector4:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, end_vect4).Ease(easeCurve));
                    case AMPropertyTrack.ValueType.Quaternion:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, new AMPlugQuaternionSlerp(end_quat)).Ease(easeCurve));
                }
            }
            else {
                switch((AMPropertyTrack.ValueType)valueType) {
                    case AMPropertyTrack.ValueType.Integer:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, Convert.ToInt32(end_val)).Ease((EaseType)easeType));
                    case AMPropertyTrack.ValueType.Float:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, Convert.ToSingle(end_val)).Ease((EaseType)easeType));
                    case AMPropertyTrack.ValueType.Double:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, new AMPlugDouble(end_val)).Ease((EaseType)easeType));
                    case AMPropertyTrack.ValueType.Long:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, new AMPlugLong(Convert.ToInt64(end_val))).Ease((EaseType)easeType));
                    case AMPropertyTrack.ValueType.Vector2:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, end_vect2).Ease((EaseType)easeType));
                    case AMPropertyTrack.ValueType.Vector3:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, end_vect3).Ease((EaseType)easeType));
                    case AMPropertyTrack.ValueType.Color:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, end_color).Ease((EaseType)easeType));
                    case AMPropertyTrack.ValueType.Rect:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, end_rect).Ease((EaseType)easeType));
                    case AMPropertyTrack.ValueType.Vector4:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, end_vect4).Ease((EaseType)easeType));
                    case AMPropertyTrack.ValueType.Quaternion:
                        return HOTween.To(component, getTime(frameRate), new TweenParms().Prop(varName, new AMPlugQuaternionSlerp(end_quat)).Ease((EaseType)easeType));
                }
            }
        }

        Debug.LogError("Animator: No FieldInfo or PropertyInfo set.");
        return null;
    }
    public override void execute(int frameRate, float delay) {
        if(targetsAreEqual()) return;
        if((endFrame == -1) || !component || ((fieldInfo == null) && (propertyInfo == null) && (methodInfo == null))) return;

        Debug.LogError("need implement");

    }

    public string getName() {
        if(fieldInfo != null) return fieldInfo.Name;
        else if(propertyInfo != null) return propertyInfo.Name;
        else if(methodInfo != null) {
        }
        return "Unknown";
    }

    public string getFloatArrayString(int codeLanguage, List<float> ls) {
        string s = "";
        if(codeLanguage == 0) s += "new float[]{";
        else s += "[";
        for(int i = 0; i < ls.Count; i++) {
            s += ls[i].ToString();
            if(codeLanguage == 0) s += "f";
            if(i < ls.Count - 1) s += ", ";
        }
        if(codeLanguage == 0) s += "}";
        else s += "]";
        return s;
    }
    public string getValueString(bool brief) {
        string s = "";
        if(AMPropertyTrack.isValueTypeNumeric(valueType)) {
            //s+= start_val.ToString();
            s += formatNumeric(start_val);
            if(!brief && endFrame != -1) s += " -> " + formatNumeric(end_val);
            //if(!brief && endFrame != -1) s += " -> "+end_val.ToString();
        }
        else if(valueType == (int)AMPropertyTrack.ValueType.Vector2) {
            s += start_vect2.ToString();
            if(!brief && endFrame != -1) s += " -> " + end_vect2.ToString();
        }
        else if(valueType == (int)AMPropertyTrack.ValueType.Vector3) {
            s += start_vect3.ToString();
            if(!brief && endFrame != -1) s += " -> " + end_vect3.ToString();
        }
        else if(valueType == (int)AMPropertyTrack.ValueType.Color) {
            //return null; 
            s += start_color.ToString();
            if(!brief && endFrame != -1) s += " -> " + end_color.ToString();
        }
        else if(valueType == (int)AMPropertyTrack.ValueType.Rect) {
            //return null; 
            s += start_rect.ToString();
            if(!brief && endFrame != -1) s += " -> " + end_rect.ToString();
        }
        return s;
    }
    // use for floats
    private string formatNumeric(float input) {
        double _input = (input < 0f ? input * -1f : input);
        if(_input < 1f) {
            if(_input >= 0.01f) return input.ToString("N3");
            else if(_input >= 0.001f) return input.ToString("N4");
            else if(_input >= 0.0001f) return input.ToString("N5");
            else if(_input >= 0.00001f) return input.ToString("N6");
            else return input.ToString();
        }
        return input.ToString("N2");
    }
    // use for doubles
    private string formatNumeric(double input) {
        double _input = (input < 0d ? input * -1d : input);
        if(_input < 1d) {
            if(_input >= 0.01d) return input.ToString("N3");
            else if(_input >= 0.001d) return input.ToString("N4");
            else if(_input >= 0.0001d) return input.ToString("N5");
            else if(_input >= 0.00001d) return input.ToString("N6");
            else return input.ToString();
        }
        return input.ToString("N2");
    }

    public bool targetsAreEqual() {
        if(valueType == (int)AMPropertyTrack.ValueType.Integer || valueType == (int)AMPropertyTrack.ValueType.Long || valueType == (int)AMPropertyTrack.ValueType.Float || valueType == (int)AMPropertyTrack.ValueType.Double)
            return start_val == end_val;
        if(valueType == (int)AMPropertyTrack.ValueType.Vector2) return (start_vect2 == end_vect2);
        if(valueType == (int)AMPropertyTrack.ValueType.Vector3) return (start_vect3 == end_vect3);
        if(valueType == (int)AMPropertyTrack.ValueType.Color) return (start_color == end_color); //return start_color.ToString()+" -> "+end_color.ToString();
        if(valueType == (int)AMPropertyTrack.ValueType.Rect) return (start_rect == end_rect); //return start_rect.ToString()+" -> "+end_rect.ToString();
        if(valueType == (int)AMPropertyTrack.ValueType.Vector4) return (start_vect4 == end_vect4);
        if(valueType == (int)AMPropertyTrack.ValueType.Quaternion) return (start_quat == end_quat);

        Debug.LogError("Animator: Invalid ValueType " + valueType);
        return false;
    }

    public object getStartValue() {
        if(valueType == (int)AMPropertyTrack.ValueType.Integer) return Convert.ToInt32(start_val);
        if(valueType == (int)AMPropertyTrack.ValueType.Long) return Convert.ToInt64(start_val);
        if(valueType == (int)AMPropertyTrack.ValueType.Float) return Convert.ToSingle(start_val);
        if(valueType == (int)AMPropertyTrack.ValueType.Double) return start_val;
        if(valueType == (int)AMPropertyTrack.ValueType.Vector2) return start_vect2;
        if(valueType == (int)AMPropertyTrack.ValueType.Vector3) return start_vect3;
        if(valueType == (int)AMPropertyTrack.ValueType.Color) return start_color; //return start_color.ToString()+" -> "+end_color.ToString();
        if(valueType == (int)AMPropertyTrack.ValueType.Rect) return start_rect; //return start_rect.ToString()+" -> "+end_rect.ToString();
        if(valueType == (int)AMPropertyTrack.ValueType.Vector4) return start_vect4;
        if(valueType == (int)AMPropertyTrack.ValueType.Quaternion) return start_quat;
        return "Unknown";
    }
    public object getEndValue() {
        if(valueType == (int)AMPropertyTrack.ValueType.Integer) return Convert.ToInt32(end_val);
        if(valueType == (int)AMPropertyTrack.ValueType.Long) return Convert.ToInt64(end_val);
        if(valueType == (int)AMPropertyTrack.ValueType.Float) return Convert.ToSingle(end_val);
        if(valueType == (int)AMPropertyTrack.ValueType.Double) return end_val;
        if(valueType == (int)AMPropertyTrack.ValueType.Vector2) return end_vect2;
        if(valueType == (int)AMPropertyTrack.ValueType.Vector3) return end_vect3;
        if(valueType == (int)AMPropertyTrack.ValueType.Color) return end_color; //return start_color.ToString()+" -> "+end_color.ToString();
        if(valueType == (int)AMPropertyTrack.ValueType.Rect) return end_rect; //return start_rect.ToString()+" -> "+end_rect.ToString();
        if(valueType == (int)AMPropertyTrack.ValueType.Vector4) return end_vect4;
        if(valueType == (int)AMPropertyTrack.ValueType.Quaternion) return end_quat;
        return "Unknown";
    }

    public override AnimatorTimeline.JSONAction getJSONAction(int frameRate) {
        if(targetsAreEqual()) return null;
        if((endFrame == -1) || !component || ((fieldInfo == null) && (propertyInfo == null) && (methodInfo == null))) return null;

        AnimatorTimeline.JSONAction a = new AnimatorTimeline.JSONAction();
        a.method = "propertyto";
        a.go = component.gameObject.name;
        a.delay = getWaitTime(frameRate, 0f);
        a.time = getTime(frameRate);
        List<string> strings = new List<string>();
        strings.Add(component.GetType().Name);
        if(fieldInfo != null || propertyInfo != null) {
            if(valueType == (int)AMPropertyTrack.ValueType.Integer) {
                strings.Add("integer");
                a.ints = new int[] { Convert.ToInt32(start_val), Convert.ToInt32(end_val) };
            }
            else if(valueType == (int)AMPropertyTrack.ValueType.Long) {
                strings.Add("long");
                a.longs = new long[] { Convert.ToInt64(start_val), Convert.ToInt64(end_val) };
            }
            else if(valueType == (int)AMPropertyTrack.ValueType.Float) {
                strings.Add("float");
                a.floats = new float[] { Convert.ToSingle(start_val), Convert.ToSingle(end_val) };
            }
            else if(valueType == (int)AMPropertyTrack.ValueType.Double) {
                strings.Add("double");
                a.doubles = new double[] { start_val, end_val };
            }
            else if(valueType == (int)AMPropertyTrack.ValueType.Vector2) {
                strings.Add("vector2");
                AnimatorTimeline.JSONVector2 v1 = new AnimatorTimeline.JSONVector2();
                v1.setValue(start_vect2);
                AnimatorTimeline.JSONVector2 v2 = new AnimatorTimeline.JSONVector2();
                v2.setValue(end_vect2);
                a.vect2s = new AnimatorTimeline.JSONVector2[] { v1, v2 };
            }
            else if(valueType == (int)AMPropertyTrack.ValueType.Vector3) {
                strings.Add("vector3");
                a.setPath(new Vector3[] { start_vect3, end_vect3 });
            }
            else if(valueType == (int)AMPropertyTrack.ValueType.Color) {
                strings.Add("color");
                AnimatorTimeline.JSONColor c1 = new AnimatorTimeline.JSONColor();
                c1.setValue(start_color);
                AnimatorTimeline.JSONColor c2 = new AnimatorTimeline.JSONColor();
                c2.setValue(end_color);
                a.colors = new AnimatorTimeline.JSONColor[] { c1, c2 };
            }
            else if(valueType == (int)AMPropertyTrack.ValueType.Rect) {
                strings.Add("rect");
                AnimatorTimeline.JSONRect r1 = new AnimatorTimeline.JSONRect();
                r1.setValue(start_rect);
                AnimatorTimeline.JSONRect r2 = new AnimatorTimeline.JSONRect();
                r2.setValue(end_rect);
                a.rects = new AnimatorTimeline.JSONRect[] { r1, r2 };
            }
            if(fieldInfo != null) {
                strings.Add("fieldinfo");
                strings.Add(fieldInfo.Name);
            }
            else {
                strings.Add("propertyinfo");
                strings.Add(propertyInfo.Name);
            }

        }
        setupJSONActionEase(a);
        a.strings = strings.ToArray();
        return a;
    }
}
