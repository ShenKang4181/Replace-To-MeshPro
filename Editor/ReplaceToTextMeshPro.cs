using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReplaceToTextMeshPro
{
    [MenuItem( "CONTEXT/Text/替换成TextMeshPro" )]
    public static void ReplaceTextToTextMeshPro( MenuCommand command )
    {
        var text = ( Text ) command.context;
        UpgradeText( text );
    }

    [MenuItem( "CONTEXT/InputField/替换成TMP_InputField" )]
    public static void ReplaceInputFieldToTMP_InputField( MenuCommand command )
    {
        var inputField = ( InputField ) command.context;
        UpgradeInputField( inputField );
    }

    [MenuItem( "CONTEXT/Dropdown/替换成TMP_Dropdown" )]
    public static void ReplaceDropdownToTMP_Dropdown( MenuCommand command )
    {
        var dropdown = ( Dropdown ) command.context;
        UpgradeDropdown( dropdown );
    }

    #region

    private class SelectableObjectProperties
    {
        private readonly AnimationTriggers animationTriggers;
        private readonly ColorBlock colors;
        private readonly Image image;
        private readonly bool interactable;
        private readonly Navigation navigation;
        private readonly SpriteState spriteState;
        private readonly Graphic targetGraphic;
        private readonly Selectable.Transition transition;

        public SelectableObjectProperties( Selectable selectable )
        {
            animationTriggers = selectable.animationTriggers;
            colors = selectable.colors;
            image = selectable.image;
            interactable = selectable.interactable;
            navigation = selectable.navigation;
            spriteState = selectable.spriteState;
            targetGraphic = selectable.targetGraphic;
            transition = selectable.transition;
        }

        public void ApplyTo( Selectable selectable )
        {
            selectable.animationTriggers = animationTriggers;
            selectable.colors = colors;
            selectable.image = image;
            selectable.interactable = interactable;
            selectable.navigation = navigation;
            selectable.spriteState = spriteState;
            selectable.targetGraphic = targetGraphic;
            selectable.transition = transition;
        }
    }

    private static FieldInfo unityEventPersistentCallsField;

    private static TextMeshProUGUI UpgradeText( Text text )
    {
        if ( !text )
            return null;

        var go = text.gameObject;
        var alignment = GetTMPAlignment( text.alignment , text.alignByGeometry );
        var bestFit = text.resizeTextForBestFit;
        var bestFitMaxSize = text.resizeTextMaxSize;
        var bestFitMinSize = text.resizeTextMinSize;
        var color = text.color;
        var enabled = text.enabled;
        //Material fontMaterial;
        //TMP_FontAsset font = GetCorrespondingTMPFontAsset( text.font , text , out fontMaterial );
        var fontSize = text.fontSize;
        var fontStyle = GetTMPFontStyle( text.fontStyle );
        var horizontalWrapMode = text.horizontalOverflow == HorizontalWrapMode.Wrap;
        var lineSpacing = ( text.lineSpacing - 1 ) * 100f;
        var raycastTarget = text.raycastTarget;
        var supportRichText = text.supportRichText;
        var _text = text.text;
        var verticalOverflow = GetTMPVerticalOverflow( text.verticalOverflow , text.horizontalOverflow );
        Object.DestroyImmediate( text , true );
        var tmp = go.AddComponent<TextMeshProUGUI>( );
        tmp.alignment = alignment;
        tmp.enableAutoSizing = bestFit;
        tmp.fontSizeMax = bestFitMaxSize;
        tmp.fontSizeMin = bestFitMinSize;
        tmp.color = color;
        tmp.enabled = enabled;
        //tmp.font = font;
        //tmp.fontMaterial = fontMaterial;
        tmp.fontSize = fontSize;
        tmp.fontStyle = fontStyle;
        tmp.enableWordWrapping = horizontalWrapMode;
        tmp.lineSpacing = lineSpacing;
        tmp.raycastTarget = raycastTarget;
        tmp.richText = supportRichText;
        tmp.text = _text;
        tmp.overflowMode = verticalOverflow;
        return tmp;
    }

    private static TMP_InputField UpgradeInputField( InputField inputField )
    {
        if ( !inputField )
            return null;

        var go = inputField.gameObject;
        var selectableProperties = new SelectableObjectProperties( inputField );
        var asteriskChar = inputField.asteriskChar;
        var caretBlinkRate = inputField.caretBlinkRate;
        var customCaretColor = inputField.customCaretColor;
        Color? caretColor = null;
        try
        {
            caretColor = inputField.caretColor;
        }
        catch { }
        float caretWidth = inputField.caretWidth;
        var characterLimit = inputField.characterLimit;
        var characterValidation = GetTMPCharacterValidation( inputField.characterValidation );
        var contentType = GetTMPContentType( inputField.contentType );
        var enabled = inputField.enabled;
        var inputType = GetTMPInputType( inputField.inputType );
        var keyboardType = inputField.keyboardType;
        var lineType = GetTMPLineType( inputField.lineType );
        var readOnly = inputField.readOnly;
        var selectionColor = inputField.selectionColor;
        var shouldHideMobileInput = inputField.shouldHideMobileInput;
        var _text = inputField.text;
        var onEndEdit = CopyUnityEvent( inputField.onEndEdit );
#if UNITY_5_3_OR_NEWER
        var onValueChanged = CopyUnityEvent( inputField.onValueChanged );
#else
		object onValueChanged = CopyUnityEvent( inputField.onValueChange );
#endif
        var textComponent = UpgradeText( inputField.textComponent );
        var placeholderComponent = ( inputField.placeholder as Text ) ? UpgradeText( ( Text ) inputField.placeholder ) : inputField.placeholder;
        Object.DestroyImmediate( inputField , true );
        var tmp = go.AddComponent<TMP_InputField>( );
        tmp.textComponent = textComponent;
        tmp.placeholder = placeholderComponent;
        selectableProperties.ApplyTo( tmp );
        tmp.asteriskChar = asteriskChar;
        tmp.caretBlinkRate = caretBlinkRate;
        tmp.customCaretColor = customCaretColor;
        try
        {
            if ( caretColor.HasValue )
                tmp.caretColor = caretColor.Value;
        }
        catch { }
        tmp.caretWidth = Mathf.RoundToInt( caretWidth );
        tmp.characterLimit = characterLimit;
        tmp.characterValidation = characterValidation;
        tmp.contentType = contentType;
        tmp.enabled = enabled;
        tmp.inputType = inputType;
        tmp.keyboardType = keyboardType;
        if ( tmp.lineType == lineType )
            tmp.lineType = ( TMP_InputField.LineType ) ( ( ( int ) lineType + 1 ) % 3 );
        tmp.lineType = lineType;
        if ( textComponent )
            textComponent.overflowMode = TextOverflowModes.Overflow;
        tmp.readOnly = readOnly;
        tmp.selectionColor = selectionColor;
        tmp.shouldHideMobileInput = shouldHideMobileInput;
        tmp.text = _text;
        PasteUnityEvent( tmp.onEndEdit , onEndEdit );
        PasteUnityEvent( tmp.onValueChanged , onValueChanged );
        if ( textComponent )
        {
            RectTransform viewport;
            if ( textComponent.transform.parent != tmp.transform )
                viewport = ( RectTransform ) textComponent.transform.parent;
            else
                viewport = CreateInputFieldViewport( tmp , textComponent , placeholderComponent );

            if ( !viewport.GetComponent<RectMask2D>( ) )
                viewport.gameObject.AddComponent<RectMask2D>( );

            tmp.textViewport = viewport;
        }

        return tmp;
    }

    private static TMP_Dropdown UpgradeDropdown( Dropdown dropdown )
    {
        if ( !dropdown )
            return null;

        var go = dropdown.gameObject;
        var selectableProperties = new SelectableObjectProperties( dropdown );
        var captionImage = dropdown.captionImage;
        var enabled = dropdown.enabled;
        var itemImage = dropdown.itemImage;
        var options = GetTMPDropdownOptions( dropdown.options );
        var template = dropdown.template;
        var value = dropdown.value;
        var onValueChanged = CopyUnityEvent( dropdown.onValueChanged );
        var captionText = UpgradeText( dropdown.captionText );
        var itemText = UpgradeText( dropdown.itemText );
        Object.DestroyImmediate( dropdown , true );
        var tmp = go.AddComponent<TMP_Dropdown>( );
        tmp.captionText = captionText;
        tmp.itemText = itemText;
        selectableProperties.ApplyTo( tmp );
        tmp.captionImage = captionImage;
        tmp.enabled = enabled;
        tmp.itemImage = itemImage;
        tmp.options = options;
        tmp.template = template;
        tmp.value = value;
        PasteUnityEvent( tmp.onValueChanged , onValueChanged );
        return tmp;
    }

    private static RectTransform CreateInputFieldViewport( TMP_InputField tmp , TextMeshProUGUI textComponent , Graphic placeholderComponent )
    {
        RectTransform viewport = null;
        try
        {
            viewport = ( RectTransform ) new GameObject( "Text Area" , typeof( RectTransform ) ).transform;
            viewport.transform.SetParent( tmp.transform , false );
            viewport.SetSiblingIndex( textComponent.rectTransform.GetSiblingIndex( ) );
            viewport.localPosition = textComponent.rectTransform.localPosition;
            viewport.localRotation = textComponent.rectTransform.localRotation;
            viewport.localScale = textComponent.rectTransform.localScale;
            viewport.anchorMin = textComponent.rectTransform.anchorMin;
            viewport.anchorMax = textComponent.rectTransform.anchorMax;
            viewport.pivot = textComponent.rectTransform.pivot;
            viewport.anchoredPosition = textComponent.rectTransform.anchoredPosition;
            viewport.sizeDelta = textComponent.rectTransform.sizeDelta;

#if UNITY_2018_3_OR_NEWER
            PrefabUtility.RecordPrefabInstancePropertyModifications( viewport.gameObject );
            PrefabUtility.RecordPrefabInstancePropertyModifications( viewport.transform );
#endif

            for ( var i = tmp.transform.childCount - 1 ; i >= 0 ; i-- )
            {
                var child = tmp.transform.GetChild( i );
                if ( child == viewport )
                    continue;

                if ( child == textComponent.rectTransform || ( placeholderComponent && child == placeholderComponent.rectTransform ) )
                {
                    child.SetParent( viewport , true );
                    child.SetSiblingIndex( 0 );

#if UNITY_2018_3_OR_NEWER
                    PrefabUtility.RecordPrefabInstancePropertyModifications( child );
#endif
                }
            }
        }
        catch
        {
            if ( viewport )
            {
                Object.DestroyImmediate( viewport );
                viewport = null;
            }

            throw;
        }

        return viewport;
    }

    private static List<TMP_Dropdown.OptionData> GetTMPDropdownOptions( List<Dropdown.OptionData> options )
    {
        if ( options == null )
            return null;

        var result = new List<TMP_Dropdown.OptionData>( options.Count );
        for ( var i = 0 ; i < options.Count ; i++ )
            result.Add( new TMP_Dropdown.OptionData( options[ i ].text , options[ i ].image ) );

        return result;
    }

    private static TMP_InputField.CharacterValidation GetTMPCharacterValidation( InputField.CharacterValidation characterValidation )
    {
        switch ( characterValidation )
        {
            case InputField.CharacterValidation.Alphanumeric:
                return TMP_InputField.CharacterValidation.Alphanumeric;
            case InputField.CharacterValidation.Decimal:
                return TMP_InputField.CharacterValidation.Decimal;
            case InputField.CharacterValidation.EmailAddress:
                return TMP_InputField.CharacterValidation.EmailAddress;
            case InputField.CharacterValidation.Integer:
                return TMP_InputField.CharacterValidation.Integer;
            case InputField.CharacterValidation.Name:
                return TMP_InputField.CharacterValidation.Name;
            case InputField.CharacterValidation.None:
                return TMP_InputField.CharacterValidation.None;
            default:
                return TMP_InputField.CharacterValidation.None;
        }
    }

    private static TMP_InputField.ContentType GetTMPContentType( InputField.ContentType contentType )
    {
        switch ( contentType )
        {
            case InputField.ContentType.Alphanumeric:
                return TMP_InputField.ContentType.Alphanumeric;
            case InputField.ContentType.Autocorrected:
                return TMP_InputField.ContentType.Autocorrected;
            case InputField.ContentType.Custom:
                return TMP_InputField.ContentType.Custom;
            case InputField.ContentType.DecimalNumber:
                return TMP_InputField.ContentType.DecimalNumber;
            case InputField.ContentType.EmailAddress:
                return TMP_InputField.ContentType.EmailAddress;
            case InputField.ContentType.IntegerNumber:
                return TMP_InputField.ContentType.IntegerNumber;
            case InputField.ContentType.Name:
                return TMP_InputField.ContentType.Name;
            case InputField.ContentType.Password:
                return TMP_InputField.ContentType.Password;
            case InputField.ContentType.Pin:
                return TMP_InputField.ContentType.Pin;
            case InputField.ContentType.Standard:
                return TMP_InputField.ContentType.Standard;
            default:
                return TMP_InputField.ContentType.Standard;
        }
    }

    private static TMP_InputField.InputType GetTMPInputType( InputField.InputType inputType )
    {
        switch ( inputType )
        {
            case InputField.InputType.AutoCorrect:
                return TMP_InputField.InputType.AutoCorrect;
            case InputField.InputType.Password:
                return TMP_InputField.InputType.Password;
            case InputField.InputType.Standard:
                return TMP_InputField.InputType.Standard;
            default:
                return TMP_InputField.InputType.Standard;
        }
    }

    private static TMP_InputField.LineType GetTMPLineType( InputField.LineType lineType )
    {
        switch ( lineType )
        {
            case InputField.LineType.MultiLineNewline:
                return TMP_InputField.LineType.MultiLineNewline;
            case InputField.LineType.MultiLineSubmit:
                return TMP_InputField.LineType.MultiLineSubmit;
            case InputField.LineType.SingleLine:
                return TMP_InputField.LineType.SingleLine;
            default:
                return TMP_InputField.LineType.SingleLine;
        }
    }

    private static TextAlignmentOptions GetTMPAlignment( TextAnchor alignment , bool alignByGeometry )
    {
        switch ( alignment )
        {
            case TextAnchor.LowerLeft:
                return alignByGeometry ? TextAlignmentOptions.BottomLeft : TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter:
                return alignByGeometry ? TextAlignmentOptions.BottomGeoAligned : TextAlignmentOptions.Bottom;
            case TextAnchor.LowerRight:
                return alignByGeometry ? TextAlignmentOptions.BottomRight : TextAlignmentOptions.BottomRight;
            case TextAnchor.MiddleLeft:
                return alignByGeometry ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.Left;
            case TextAnchor.MiddleCenter:
                return alignByGeometry ? TextAlignmentOptions.MidlineGeoAligned : TextAlignmentOptions.Center;
            case TextAnchor.MiddleRight:
                return alignByGeometry ? TextAlignmentOptions.MidlineRight : TextAlignmentOptions.Right;
            case TextAnchor.UpperLeft:
                return alignByGeometry ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter:
                return alignByGeometry ? TextAlignmentOptions.TopGeoAligned : TextAlignmentOptions.Top;
            case TextAnchor.UpperRight:
                return alignByGeometry ? TextAlignmentOptions.TopRight : TextAlignmentOptions.TopRight;
            default:
                return alignByGeometry ? TextAlignmentOptions.MidlineGeoAligned : TextAlignmentOptions.Center;
        }
    }

    private static FontStyles GetTMPFontStyle( FontStyle fontStyle )
    {
        switch ( fontStyle )
        {
            case FontStyle.Bold:
                return FontStyles.Bold;
            case FontStyle.Italic:
                return FontStyles.Italic;
            case FontStyle.BoldAndItalic:
                return FontStyles.Bold | FontStyles.Italic;
            default:
                return FontStyles.Normal;
        }
    }

    private static TextOverflowModes GetTMPVerticalOverflow( VerticalWrapMode verticalOverflow , HorizontalWrapMode horizontalOverflow )
    {
        return verticalOverflow == VerticalWrapMode.Overflow ? TextOverflowModes.Overflow : TextOverflowModes.Truncate;
    }

    private static object CopyUnityEvent( UnityEventBase target )
    {
        if ( unityEventPersistentCallsField == null )
        {
            unityEventPersistentCallsField = typeof( UnityEventBase ).GetField( "m_PersistentCalls" , BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
            if ( unityEventPersistentCallsField == null )
            {
                unityEventPersistentCallsField = typeof( UnityEventBase ).GetField( "m_PersistentListeners" , BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );

                if ( unityEventPersistentCallsField == null )
                {
                    Debug.LogError( "无法复制UnityEvent!" );
                    return null;
                }
            }
        }

        return unityEventPersistentCallsField.GetValue( target );
    }

    private static void PasteUnityEvent( UnityEventBase target , object unityEvent )
    {
        unityEventPersistentCallsField.SetValue( target , unityEvent );
    }

    #endregion
}