/**************************************************************
* Programador: Leandro Dornela Ribeiro
* Contato: leandrodornela@ice.ufjf.br
* Data de criação: 02/2020
//************************************************************/


using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public enum AnimationSide
{
    top,
    botton,
    left,
    right
}


/// <summary>
/// [pt-BR] - Classe para animação de um RectTransform em um canvas para ocultar e exibir janelas.
/// </summary>
public class AnimatedRect : MonoBehaviour
{
    [TextArea(3,5)] [SerializeField] private string instructions = "[pt-BR] - O RectTransform window deve ser preferencialmente um filho do objeto com este script.";

    [Header("Animation")]

    [Tooltip("[pt-BR] - Rect da janela que será movida.")]
    [SerializeField] private RectTransform window;

    [Tooltip("[pt-BR] - Sentido da animação.")]
    [SerializeField] private AnimationSide animationSide;

    [Tooltip("[pt-BR] - ")]
    [SerializeField] AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    [Tooltip("[pt-BR] - ")]
    [SerializeField] private float animationDuration = 1;

    [Tooltip("[pt-BR] - Parar o tempo quando a janela estiver ativa.")]
    [SerializeField] private bool pauseTimeIfOpen = true;

    [Tooltip("[pt-BR] - Ocultar a janela para a posição inicial.")]
    [SerializeField] private bool hideOnAwake = false;


    [Header("Events")]

    [SerializeField] private UnityEvent onOpen;
    [SerializeField] private UnityEvent onOpened;
    [SerializeField] private UnityEvent onClose;
    [SerializeField] private UnityEvent onClosed;


    // Posição da janela quando ativa.
    private Vector3 activePosition = Vector3.zero;

    // Posição da janela quando oculta.
    private Vector3 hidenPosition = new Vector3(0, 0, 0);

    // Posição alvo da janela.
    private Vector3 targetPosition = Vector3.zero;

    // Ultima posição do alvo.
    private Vector3 lastPosition = Vector3.zero;

    // Verdadeiro quando a animação de abertura está em andamento.
    private bool openAnimation = false;

    // Verdadeiro quando a animação de fechamento está em andamento.
    private bool closeAnimation = false;

    // Verdadeiro quando a janela está aberta.
    private bool isOpen = false;

    // Referência ao CanvasScaler.
    private CanvasScaler canvasScaler;

    // Resolução de referência para o canvas.
    private Vector2 referenceResolution = new Vector2(1920, 1080);

    // Contador de tempo para a animação.
    private float animationTimer = 0;


    private void Awake()
    {
        canvasScaler = GetComponentInParent<CanvasScaler>();

        if(canvasScaler == null)
        {
            Debug.LogWarning("[en-US] - AnimatedRect needs a CanvasScaler on parent to work.");
        }
        else
        {
            referenceResolution = canvasScaler.referenceResolution;
        }


        switch(animationSide)
        {
            case AnimationSide.top:
                hidenPosition.y = (referenceResolution.y / 2 + window.sizeDelta.y);
                break;
            case AnimationSide.botton:
                hidenPosition.y = -(referenceResolution.y / 2 + window.sizeDelta.y);
                break;
            case AnimationSide.left:
                hidenPosition.x = -(referenceResolution.x / 2 + window.sizeDelta.x);
                break;
            case AnimationSide.right:
                hidenPosition.x = (referenceResolution.x / 2 + window.sizeDelta.x);
                break;
        }


        if (hideOnAwake)
        {
            window.gameObject.SetActive(false);
            targetPosition = hidenPosition;
            window.localPosition = targetPosition;
        }
        else
        {
            isOpen = true;
        }
    }


    void Update()
    {
        if (openAnimation || closeAnimation)
        {
            MoveWindow();
        }
    }


    /// <summary>
    /// [pt-BR] - Move a janela até que atinja a posição alvo.
    /// </summary>
    void MoveWindow()
    {
        float percent = Mathf.Clamp01(animationTimer / animationDuration);
        float curvePercent = animationCurve.Evaluate(percent);
        window.localPosition = Vector3.LerpUnclamped(lastPosition, targetPosition, curvePercent);

        animationTimer += Time.unscaledDeltaTime;

        if (animationTimer >= animationDuration)
        {
            animationTimer = 0;

            if (openAnimation)
            {
                EndOfOpenAnimation();
            }
            else if (closeAnimation)
            {
                EndOfCloseAnimation();
            }
        }
    }


    /// <summary>
    /// [pt-BR] - Desativa a janela e chama as callbacks quando finaliza a animação de fechamento.
    /// </summary>
    void EndOfCloseAnimation()
    {
        closeAnimation = false;

        if (pauseTimeIfOpen) Time.timeScale = 1;

        onClosed.Invoke();

        window.gameObject.SetActive(false);
    }


    /// <summary>
    /// [pt-BR] - Ações para fim da animação de abertura da janela.
    /// </summary>
    void EndOfOpenAnimation()
    {
        openAnimation = false;
        onOpened.Invoke();
    }


    /// <summary>
    /// [pt-BR] - 
    /// </summary>
    public void Open()
    {
        if(isOpen)
        {
            return;
        }

        if (pauseTimeIfOpen) Time.timeScale = 0;
        lastPosition = hidenPosition;
        targetPosition = activePosition;
        window.gameObject.SetActive(true);
        openAnimation = true;
        closeAnimation = false;
        isOpen = true;

        onOpen.Invoke();
    }


    /// <summary>
    /// [pt-BR] - 
    /// </summary>
    public void Close()
    {
        if(closeAnimation)
        {
            return;
        }

        closeAnimation = true;
        openAnimation = false;
        lastPosition = activePosition;
        targetPosition = hidenPosition;
        isOpen = false;

        onClose.Invoke();
    }
}
