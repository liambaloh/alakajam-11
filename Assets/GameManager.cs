using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _layer1;
        [SerializeField] private Image _layer2;
        [SerializeField] private Image _layer3;

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextMeshProUGUI _option1;
        [SerializeField] private TextMeshProUGUI _option2;
        [SerializeField] private TextMeshProUGUI _option3;
        [SerializeField] private TextMeshProUGUI _option4;

        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioSource _ambiance1AudioSource;
        [SerializeField] private AudioSource _ambiance2AudioSource;

        [SerializeField] private Dictionary<string, bool> _variables;

        private Dictionary<string, AudioClip> _songs;
        private List<SceneDescriptor> _scenes;

        private List<Sequence> _sequences;

        void Awake()
        {
            _variables = new Dictionary<string, bool>();
            _songs = new Dictionary<string, AudioClip>() {{"", null}};
            _scenes = new List<SceneDescriptor>();
            _sequences = new List<Sequence>();
        }

        void Start()
        {
            DOTween.Init();
            var info = new DirectoryInfo(Application.streamingAssetsPath + "/Scenes");
            var fileInfo = info.GetFiles();
            foreach (var file in fileInfo)
            {
                if (file.Extension.ToLower() == ".json")
                {
                    var reader = new StreamReader(Application.streamingAssetsPath + "/Scenes/" + file.Name);
                    var json = reader.ReadToEnd();
                    reader.Close();
                    var sceneDescriptor = JsonUtility.FromJson<SceneDescriptor>(json);
                    _scenes.Add(sceneDescriptor);

                }
            }

            GetSongsFromFolder();

            ApplyScene("main");
        }

        void Update()
        {
            var backgroundContainerRT = (RectTransform) _background.rectTransform.parent;

            UpdateImageScaling(backgroundContainerRT, _background);
            UpdateImageScaling(backgroundContainerRT, _layer1);
            UpdateImageScaling(backgroundContainerRT, _layer2);
            UpdateImageScaling(backgroundContainerRT, _layer3);

            _text.rectTransform.sizeDelta = new Vector2(0, _text.preferredHeight);
        }

        public void ApplyScene(string sceneName)
        {
            foreach (var scene in _scenes)
            {
                if (scene.id == sceneName)
                {
                    foreach (var variable in scene.variables_set)
                    {
                        _variables[variable] = true;
                    }

                    foreach (var variable in scene.variables_unset)
                    {
                        _variables[variable] = false;
                    }

                    foreach (var sequence1 in _sequences)
                    {
                        sequence1.Kill();
                    }
                    _sequences.Clear();

                    LoadGraphic(scene.background, _background);
                    LoadGraphic(scene.layer1, _layer1);
                    LoadGraphic(scene.layer2, _layer2);
                    LoadGraphic(scene.layer3, _layer3);

                    _text.text = scene.text;
                    _text.rectTransform.sizeDelta = new Vector2(0, _text.preferredHeight);

                    SetInteraction(scene.interactions[0], _option1);
                    SetInteraction(scene.interactions[1], _option2);
                    SetInteraction(scene.interactions[2], _option3);
                    SetInteraction(scene.interactions[3], _option4);

                    LoadSound(scene.music, _musicAudioSource);

                    if (string.IsNullOrEmpty(scene.ambiance1.track))
                    {
                        if (string.IsNullOrEmpty(scene.ambiance2.track))
                        {
                            //both empty
                            //no op
                        }
                        else
                        {
                            //1 empty, 2 set
                            LoadSound(scene.ambiance2, _ambiance1AudioSource);
                        }
                    }
                    else
                    {
                        //both set
                        if (_ambiance1AudioSource.clip == _songs[scene.ambiance1.track])
                        {
                            LoadSound(scene.ambiance1, _ambiance1AudioSource);
                            LoadSound(scene.ambiance2, _ambiance2AudioSource);
                        }
                        else
                        {
                            LoadSound(scene.ambiance2, _ambiance1AudioSource);
                            LoadSound(scene.ambiance1, _ambiance2AudioSource);
                        }
                    }
                }
            }
        }

        private void LoadGraphic(AnimatedImageDescriptor imageDescriptor, Image image)
        {
            if (imageDescriptor.image != "")
            {
                var imagePath = Application.streamingAssetsPath + "/Backgrounds/" + imageDescriptor.image;
                var sprite = LoadNewSprite(imagePath);
                image.sprite = sprite;
                image.gameObject.SetActive(true);
                if (imageDescriptor.aniamtes)
                {
                    image.color = new Color(1, 1, 1, 0);
                    var sequence = DOTween.Sequence()
                        .Append(image.DOFade(1f, imageDescriptor.fadeInTime))
                        .AppendInterval(imageDescriptor.sustainTime)
                        .Append(image.DOFade(0f, imageDescriptor.fadeOutTime))
                        .AppendInterval(imageDescriptor.repeatDelay)
                        .Pause();
                    if (imageDescriptor.loop)
                    {
                        sequence.SetLoops(-1);
                    }

                    var initialSequence = DOTween.Sequence()
                        .AppendInterval(imageDescriptor.initialDelay)
                        .OnComplete(() => { sequence.Play(); });
                    initialSequence.Play();

                    _sequences.Add(sequence);
                    _sequences.Add(initialSequence);
                }
            }
            else
            {
                image.gameObject.SetActive(false);
            }
        }

        private void LoadSound(MusicDescriptor clip, AudioSource source)
        {
            if (string.IsNullOrEmpty(clip.track))
            {
                source.clip = null;
                source.Stop();
            }
            else
            {
                source.loop = clip.looping;
                if (source.clip != _songs[clip.track])
                {
                    source.volume = clip.volume / 100f;
                    source.clip = _songs[clip.track];
                    source.Play();
                }
                else
                {
                    source.DOFade(clip.volume / 100f, 2f);
                }
            }
        }

        private void UpdateImageScaling(RectTransform container, Image image)
        {
            var containerHeight = container.rect.height;
            var containerWidth = container.rect.width;

            var sprite = image.sprite;
            if (sprite != null)
            {
                var width = sprite.rect.width;
                var height = sprite.rect.height;

                var fitToHeightHeight = containerHeight;
                var fitToHeightWidth = (containerHeight / height) * width;
                var fitToWidthHeight = (containerWidth / width) * height;
                var fitToWidthWidth = containerWidth;

                var fitHeight = containerHeight;
                var fitWidth = containerWidth;

                if ((fitToHeightHeight < fitToWidthHeight - 0.1f) || (fitToHeightWidth < fitToWidthWidth - 0.1f))
                {
                    fitHeight = fitToHeightHeight;
                    fitWidth = fitToHeightWidth;
                }
                else if ((fitToWidthHeight < fitToHeightHeight - 0.1f) || (fitToWidthWidth < fitToHeightWidth - 0.1f))
                {
                    fitHeight = fitToWidthHeight;
                    fitWidth = fitToWidthWidth;
                }

                image.rectTransform.sizeDelta = new Vector2(fitWidth, fitHeight);
            }
        }

        private void SetInteraction(InteractionDescriptor interaction, TextMeshProUGUI text)
        {
            if (string.IsNullOrEmpty(interaction.text))
            {
                text.gameObject.SetActive(false);
                return;
            }

            foreach (var variable in interaction.requires_variables.set)
            {
                if (!_variables.ContainsKey(variable) || _variables[variable] == false)
                {
                    text.gameObject.SetActive(false);
                    return;
                }
            }

            foreach (var variable in interaction.requires_variables.unset)
            {
                if (_variables.ContainsKey(variable) && _variables[variable] == true)
                {
                    text.gameObject.SetActive(false);
                    return;
                }
            }

            text.text = interaction.text;
            var button = text.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                var totalWeight = 0f;
                foreach (var exitSceneDescriptor in interaction.exit_scenes)
                {
                    totalWeight += exitSceneDescriptor.probability_weight;
                }

                var selectedWeight = Random.Range(0f, totalWeight);
                foreach (var exitSceneDescriptor in interaction.exit_scenes)
                {
                    selectedWeight -= exitSceneDescriptor.probability_weight;
                    if (selectedWeight <= 0)
                    {
                        if (exitSceneDescriptor.exit_game)
                        {
                            Application.Quit();
                        }
                        else
                        {
                            if (exitSceneDescriptor.variables_set != null)
                            {
                                foreach (var variable in exitSceneDescriptor.variables_set)
                                {
                                    _variables[variable] = true;
                                }
                            }
                            
                            if (exitSceneDescriptor.variables_unset != null)
                            {
                                foreach (var variable in exitSceneDescriptor.variables_unset)
                                {
                                    _variables[variable] = false;
                                }
                            }
                            ApplyScene(exitSceneDescriptor.scene);
                        }

                        return;
                    }
                }
            });
            text.gameObject.SetActive(true);
        }

        public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f,
            SpriteMeshType spriteType = SpriteMeshType.Tight)
        {
            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

            Texture2D SpriteTexture = LoadTexture(FilePath);
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),
                new Vector2(0, 0), PixelsPerUnit, 0, spriteType);

            return NewSprite;
        }

        public static Sprite ConvertTextureToSprite(Texture2D texture, float PixelsPerUnit = 100.0f,
            SpriteMeshType spriteType = SpriteMeshType.Tight)
        {
            // Converts a Texture2D to a sprite, assign this texture to a new sprite and return its reference

            Sprite NewSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0),
                PixelsPerUnit, 0, spriteType);

            return NewSprite;
        }

        public static Texture2D LoadTexture(string FilePath)
        {
            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails

            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(2, 2); // Create new "empty" texture
                if (Tex2D.LoadImage(FileData)) // Load the imagedata into the texture (size is set automatically)
                    return Tex2D; // If data = readable -> return texture
            }

            return null; // Return null if load failed
        }

        private void GetSongsFromFolder()
        {
            var directoryInfo = new DirectoryInfo(Application.streamingAssetsPath + "/Music");
            var songFiles = directoryInfo.GetFiles("*.*");

            foreach (FileInfo songFile in songFiles)
            {
                ConvertFilesToAudioClip(songFile);
            }
        }

        private void ConvertFilesToAudioClip(FileInfo songFile)
        {
            if (songFile.Name.Contains("meta"))
                return;
            else
            {
                var songName = songFile.FullName.ToString();
                var url = string.Format("file://{0}", songName);
                var www = new WWW(url);
                while (!www.isDone)
                {
                    //doot dee doot dee doot dee
                }

                _songs.Add(songFile.Name, www.GetAudioClip(false, false));
            }
        }
    }
}