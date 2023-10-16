using Core.Business;
using Core.EventSignal;
using Core.Extension;
using Core.Framework;
using Core.Module;
using Core.Utility;
using MessagePipe;
using Microsoft.MixedReality.Toolkit.UX;
using Shared;
using Shared.Extension;
using Shared.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Core.View
{
    [System.Serializable]
    public class RoomStatusPerson
    {
        [DebugOnly] public PressableButton Button;
        [DebugOnly] public TextMeshProUGUI NameTxt;
        [DebugOnly] public Image IconImg;
    }

    [System.Serializable]
    public abstract class SubView
    {
        [DebugOnly] public Transform Transform;
        [SerializeField][DebugOnly] protected PressableButton _backBtn;
        [SerializeField][DebugOnly] protected IObjectResolver _container;

        public SubView(Transform transform, IObjectResolver container)
        {
            Transform = transform;
            _container = container;
        }

        public virtual void RegisterEvents()
        {
            _backBtn.OnClicked.AddListener(() =>
            {
                Transform.SetActive(false);
            });
        }
    }

    public class RoomStatusView : UnityView
    {
        [System.Serializable]
        public class SelectToolView : SubView
        {
            private readonly GameStore _gameStore;
            private readonly ClassRoomHub _classRoomHub;
            private readonly QuizzesHub _quizzesHub;
            private readonly IUserDataController _userDataController;

            [SerializeField][DebugOnly] protected PressableButton[] _toolBtns;

            public SelectToolView(Transform transform, IObjectResolver container) : base(transform, container)
            {
                _gameStore = container.Resolve<GameStore>();
                _classRoomHub = container.Resolve<ClassRoomHub>();
                _quizzesHub = container.Resolve<QuizzesHub>();
                _userDataController = container.Resolve<IUserDataController>();

                _backBtn = Transform.Find("Content/Header/Back_Btn").GetComponent<PressableButton>();

                _toolBtns = new PressableButton[] {
                    Transform.Find("Content/Scroll View/Viewport/Content").GetChild(0).GetComponent<PressableButton>()
                };

                RegisterEvents();
            }

            public override void RegisterEvents()
            {
                base.RegisterEvents();

                _toolBtns[0].OnClicked.AddListener(async () =>
                {
                    QuizzesStatusResponse response = await _quizzesHub.JoinAsync(new JoinQuizzesData());
                    await _classRoomHub.InviteToGame(response);

                    if (_gameStore.CheckShowToastIfNotSuccessNetwork(response))
                        return;

                    _userDataController.ServerData.RoomStatus.InGameStatus = response;

                    _gameStore.GState.RemoveModel<RoomStatusModel>();
                    await _gameStore.GetOrCreateModule<QuizzesRoomStatus, QuizzesRoomStatusModel>(
                        "", ViewName.Unity, ModuleName.QuizzesRoomStatus);
                });
            }
        }

        [System.Serializable]
        public class EditAvatarView : SubView
        {
            private readonly IBundleLoader _bundleLoader;
            private readonly ClassRoomHub _classRoomHub;
            private IUserDataController _userDataController;

            [SerializeField][DebugOnly] protected MRTKTMPInputField _nameInput;
            [SerializeField][DebugOnly] protected PressableButton[] _avatarBtns;
            [SerializeField][DebugOnly] protected PressableButton[] _modelBtns;
            [SerializeField][DebugOnly] protected PressableButton _submitBtn;

            public string SelectedAvatarPath => Defines.PrefabKey.AvatarPaths[_selectedAvatarIdx];
            [SerializeField][DebugOnly] protected int _selectedAvatarIdx = 0;
            public string SelectedModelPath => Defines.PrefabKey.ModelPaths[_selectedModelIdx];
            [SerializeField][DebugOnly] protected int _selectedModelIdx = 0;

            [SerializeField] protected Color _selectedColor = new(1f, 1f, 1f, 1f);
            [SerializeField] protected Color _defaultColor = "27984C".HexToColor();

            public EditAvatarView(Transform transform, IObjectResolver container) : base(transform, container)
            {
                _classRoomHub = container.Resolve<ClassRoomHub>();
                _bundleLoader = container.Resolve<IReadOnlyList<IBundleLoader>>().ElementAt((int)BundleLoaderName.Addressable);
                _userDataController = container.Resolve<IUserDataController>();

                _backBtn = Transform.Find("Content/Header/Back_Btn").GetComponent<PressableButton>();

                _nameInput = Transform.Find("Content/Form/InputField/InputField (TMP)").GetComponent<MRTKTMPInputField>();

                Transform parent = Transform.Find("Content/Form/Avatar_SV/Viewport/Content");
                _avatarBtns = Defines.PrefabKey.AvatarPaths.Select((path, idx) =>
                {
                    if (idx > 0)
                        Instantiate(parent.GetChild(0), parent);
                    return parent.GetChild(idx).GetComponent<PressableButton>();
                }).ToArray();

                parent = Transform.Find("Content/Form/Model_SV/Viewport/Content");
                _modelBtns = Defines.PrefabKey.ModelThumbnailPaths.Select((path, idx) =>
                {
                    if (idx > 0)
                        Instantiate(parent.GetChild(0), parent);
                    return parent.GetChild(idx).GetComponent<PressableButton>();
                }).ToArray();

                SetAvatarIcon();

                RegisterEvents();
            }

            private void EnableAvatar(int index)
            {
                _avatarBtns[_selectedAvatarIdx].transform.Find("Backplate").GetComponent<Image>().color = _defaultColor;
                _selectedAvatarIdx = index;
                _avatarBtns[_selectedAvatarIdx].transform.Find("Backplate").GetComponent<Image>().color = _selectedColor;
            }

            private void EnableModel(int index)
            {
                _modelBtns[_selectedModelIdx].transform.Find("Backplate").GetComponent<Image>().color = _defaultColor;
                _selectedModelIdx = index;
                _modelBtns[_selectedModelIdx].transform.Find("Backplate").GetComponent<Image>().color = _selectedColor;
            }

            private async void SetAvatarIcon()
            {
                _selectedAvatarIdx = Defines.PrefabKey.AvatarPaths.Select((path, idx) => (path, idx)).Where(ele => ele.path == _userDataController.ServerData.RoomStatus.RoomStatus.Self.AvatarPath).First().idx;

                for (int idx = 0; idx < _avatarBtns.Length; idx++)
                {
                    _avatarBtns[idx].transform.Find("Frontplate/AnimatedContent/Icon/UIButtonSpriteIcon").GetComponent<Image>().sprite = (await _bundleLoader.LoadAssetAsync<Texture2D>(Defines.PrefabKey.AvatarPaths[idx])).TexToSprite();

                    EnableAvatar(idx);
                }
                EnableAvatar(_selectedAvatarIdx);

                for (int idx = 0; idx < _modelBtns.Length; idx++)
                {
                    _modelBtns[idx].transform.Find("Frontplate/AnimatedContent/Icon/UIButtonSpriteIcon").GetComponent<Image>().sprite = (await _bundleLoader.LoadAssetAsync<Texture2D>(Defines.PrefabKey.ModelThumbnailPaths[idx])).TexToSprite();

                    EnableModel(idx);
                }
                EnableModel(_selectedAvatarIdx);
            }

            public override void RegisterEvents()
            {
                base.RegisterEvents();

                for (int idx = 0; idx < _avatarBtns.Length; idx++)
                {
                    int index = idx;
                    _avatarBtns[idx].OnClicked.AddListener(() =>
                    {
                        EnableAvatar(index);
                    });
                }

                for (int idx = 0; idx < _modelBtns.Length; idx++)
                {
                    int index = idx;
                    _modelBtns[idx].OnClicked.AddListener(() =>
                    {
                        EnableModel(index);
                    });
                }

                _submitBtn.OnClicked.AddListener(async () =>
                {
                    await _classRoomHub.UpdateAvatar(_nameInput.text.IsNullOrEmpty() ? "Name" : _nameInput.text, SelectedModelPath, SelectedAvatarPath);
                    Transform.SetActive(false);
                });
            }
        }

        [System.Serializable]
        public class SettingView : SubView
        {
            [SerializeField][DebugOnly] protected MRTKTMPInputField _roomCapInput;

            public SettingView(Transform transform, IObjectResolver container) : base(transform, container)
            {
                _backBtn = Transform.Find("Content/Header/Back_Btn").GetComponent<PressableButton>();

                _roomCapInput = Transform.Find("Content/Form/InputField/InputField (TMP)").GetComponent<MRTKTMPInputField>();

                RegisterEvents();
            }

            public override void RegisterEvents()
            {
                _backBtn.OnClicked.AddListener(() => Transform.SetActive(false));
            }
        }

        private IObjectResolver _container;
        private GameStore _gameStore;
        private AudioPoolManager _audioPoolManager;
        private VirtualRoomPresenter _virtualRoomPresenter;
        private IUserDataController _userDataController;
        private ClassRoomHub _classRoomHub;

        [SerializeField][DebugOnly] private PressableButton _closeBtn;
        [SerializeField][DebugOnly] private PressableButton _quitBtn;

        [SerializeField][DebugOnly] private TextMeshProUGUI _titleTxt;
        [SerializeField][DebugOnly] private TextMeshProUGUI _amountTxt;
        [SerializeField][DebugOnly] private RoomStatusPerson[] _personItems;

        [SerializeField][DebugOnly] private PressableButton _selectToolBtn;
        [SerializeField][DebugOnly] private PressableButton _editAvatarBtn;
        [SerializeField][DebugOnly] private PressableButton _settingBtn;

        [SerializeField][DebugOnly] private EditAvatarView _editAvatarView;
        [SerializeField][DebugOnly] private SelectToolView _selectToolView;
        [SerializeField][DebugOnly] private SettingView _settingView;

        [Inject]
        public void Init(
            GameStore gameStore,
            IObjectResolver container)
        {
            _container = container;
            _gameStore = gameStore;
            _audioPoolManager = (AudioPoolManager)container.Resolve<IReadOnlyList<IPoolManager>>().ElementAt((int)PoolName.Audio);
            _virtualRoomPresenter = container.Resolve<VirtualRoomPresenter>();
            _userDataController = container.Resolve<IUserDataController>();
            _classRoomHub = container.Resolve<ClassRoomHub>();
        }

        private void GetReferences()
        {
            _closeBtn = transform.Find("CanvasDialog/Canvas/Header/Close_Btn").GetComponent<PressableButton>();
            _quitBtn = transform.Find("CanvasDialog/Canvas/Header/Quit_Btn").GetComponent<PressableButton>();

            _titleTxt = transform.Find("CanvasDialog/Canvas/Header/Content/Title").GetComponent<TextMeshProUGUI>();
            _amountTxt = transform.Find("CanvasDialog/Canvas/Header/Content/Amount").GetComponent<TextMeshProUGUI>();
            var list = transform.Find("CanvasDialog/Canvas/Content/Scroll View/Viewport/Content");
            _personItems = new bool[list.childCount].Select((_, idx) =>
            {
                Transform person = list.GetChild(idx);
                return new RoomStatusPerson
                {
                    Button = person.GetComponent<PressableButton>(),
                    NameTxt = person.Find("Frontplate/AnimatedContent/Text").GetComponent<TextMeshProUGUI>(),
                    IconImg = person.Find("Frontplate/AnimatedContent/Icon/UIButtonSpriteIcon").GetComponent<Image>(),
                };
            }).ToArray();

            _selectToolBtn = transform.Find("CanvasDialog/Canvas/Footer/SelectTool_Btn").GetComponent<PressableButton>();
            _editAvatarBtn = transform.Find("CanvasDialog/Canvas/Footer/EditAvatar_Btn").GetComponent<PressableButton>();
            _settingBtn = transform.Find("CanvasDialog/Canvas/Footer/Setting_Btn").GetComponent<PressableButton>();

            _selectToolView = new SelectToolView(transform.Find("CanvasDialog/Canvas/ToolSelection"), _container);
            _editAvatarView = new EditAvatarView(transform.Find("CanvasDialog/Canvas/EditAvatar"), _container);
            _settingView = new SettingView(transform.Find("CanvasDialog/Canvas/Setting"), _container);

            _selectToolView.Transform.SetActive(false);
            _editAvatarView.Transform.SetActive(false);
            _settingView.Transform.SetActive(false);
        }

        private void RegisterEvents()
        {
            _closeBtn.OnClicked.AddListener(() =>
            {
                _gameStore.HideCurrentModule(ModuleName.RoomStatus);
            });
            _quitBtn.OnClicked.AddListener(() =>
            {
                _showPopupPublisher.Publish(new ShowPopupSignal(title: "Are you sure you want to quit the class room?", yesContent: "Yes", noContent: "No", yesAction: async (value1, value2) =>
                {
                    _showLoadingPublisher.Publish(new ShowLoadingSignal());
                    await _classRoomHub.LeaveAsync();

                    _gameStore.GState.RemoveModel<RoomStatusModel>();
                    await _gameStore.GetOrCreateModule<LandingScreen, LandingScreenModel>(
                        "", ViewName.Unity, ModuleName.LandingScreen);

                    _showLoadingPublisher.Publish(new ShowLoadingSignal(isShow: false));
                }, noAction: (_, _) => { }));
            });

            for (int idx = 0; idx < _personItems.Length; idx++)
            {
                int index = idx;
                _personItems[index].Button.OnClicked.AddListener(() =>
                {
                    Debug.Log($"{_personItems[index].NameTxt.text} - {index}");
                });
            }

            _selectToolBtn.OnClicked.AddListener(() =>
                _selectToolView.Transform.SetActive(true));
            _editAvatarBtn.OnClicked.AddListener(() =>
                _editAvatarView.Transform.SetActive(true));
            _settingBtn.OnClicked.AddListener(() =>
                _settingView.Transform.SetActive(true));
        }

        public override void OnReady()
        {
            GetReferences();
            RegisterEvents();

            Refresh();
        }

        public void Refresh()
        {
            if (!_userDataController.ServerData.IsInRoom)
            {
                _gameStore.GState.RemoveModel<RoomStatusModel>();
                return;
            }

            bool isInGame = _userDataController.ServerData.IsInGame;
            GeneralRoomStatusResponse status = _userDataController.ServerData.RoomStatus.RoomStatus;

            string idPrefix = isInGame ? "PIN" : "Room Id";
            _titleTxt.SetText($"{idPrefix}: {status.Id}");
            _amountTxt.SetText($"Amount: {status.Amount}");
            for (int idx = 0; idx < _personItems.Length; idx++)
            {
                _personItems[idx].Button.SetActive(idx >= status.Others.Length);
                if (idx >= status.Others.Length) continue;

                if (status.Others[idx].IsHost)
                {
                    idx--;
                    continue;
                }

                _personItems[idx].NameTxt.text = status.Others[idx].Name;
            }

            bool isHost = _userDataController.ServerData.RoomStatus.RoomStatus.Self.IsHost;
            _selectToolBtn.SetActive(isHost && !isInGame);
            _editAvatarBtn.SetActive(!isInGame);
            _settingBtn.SetActive(isHost && !isInGame);
        }
    }
}
