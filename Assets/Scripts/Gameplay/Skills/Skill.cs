using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Skill : MonoBehaviour
{
    [Header("Skill data")]
    public KeyCode UseKeybind;
    public SkillData Data;
    protected bool IsSkillActive;
    protected float CooldownTimer;
    public float CooldownMultiplier = 1;

    [Header("Cooldown")]
    [SerializeField] TextMeshProUGUI _cooldownText;
    bool _isCooldown = false;

    [Header("FX")]
    AudioSource _audio;
    [SerializeField] GameObject _particlesFX;
    [SerializeField] AudioClip _onUseSFX;
    [SerializeField] AudioClip _onEndSFX;

    Image _image;
    Vector3 _startScale;

    ParticleSystem _particles;

    float _scaleAnim;

    private void Awake()
    {
        _startScale = GetComponent<RectTransform>().localScale;
        _image = GetComponent<Image>();
        _audio = GetComponent<AudioSource>();
        CooldownTimer = Data.Cooldown;
    }

    protected virtual void OnSkillStart()
    {
        CooldownTimer = Data.Cooldown * CooldownMultiplier;

        if (_particlesFX != null)
        {
            var player = PlayerController.Instance.transform;
            _particles = Instantiate(_particlesFX, player).GetComponent<ParticleSystem>();

            Destroy(_particles.gameObject, Data.Duration + 5);
        }

        if (_onUseSFX) _audio.PlayOneShot(_onUseSFX, 0.7f);
    }
    public virtual void OnSkillUpdate() { }
    protected virtual void OnSkillEnd()
    {
        IsSkillActive = false;
        _isCooldown = true;
        _particles.Stop();
        if (_onEndSFX) _audio.PlayOneShot(_onEndSFX, 0.7f);
    }

    private void Update()
    {
        if (Input.GetKeyUp(UseKeybind) && !IsSkillActive && !_isCooldown && Player.Instance.IsAlive)
        {
            PlayerSkills.Instance.UseSkill(Data);
            IsSkillActive = true;
            OnSkillStart();
        }

        if (IsSkillActive)
        {
            OnSkillUpdate();
            _scaleAnim = Mathf.Sin(Time.time * 5) * 0.05f;
        }

        transform.localScale = IsSkillActive ? _startScale + (Vector3.one * _scaleAnim) : _startScale;

        if (_isCooldown)
        {
            CooldownTimer -= Time.deltaTime;

            if (CooldownTimer <= 0) _isCooldown = false;
        }

        _cooldownText.text = _isCooldown ? ((int)CooldownTimer).ToString() : "";

        _image.color = _isCooldown ? new Color(1, 1, 1, 0.1f) : Color.white;
    }
}