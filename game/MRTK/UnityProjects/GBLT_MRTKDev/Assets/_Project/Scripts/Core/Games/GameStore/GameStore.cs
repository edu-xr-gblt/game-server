﻿using Core.Business;
using Core.EventSignal;
using Core.Module;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Shared.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using VContainer;
using VContainer.Unity;

namespace Core.Framework
{
    public partial class GameStore : IStartable, IDisposable
    {
        private readonly Setting _gameSetting;
        private readonly IObjectResolver _container;
        private readonly IDefinitionDataController _definitionDataController;

        [Inject]
        private readonly IPublisher<GameScreenForceChangeSignal> _gameScreenForceChangePublisher;

        [Inject]
        private readonly IPublisher<GameScreenChangeSignal> _gameScreenChangePublisher;

        [Inject]
        protected readonly IPublisher<ShowLoadingSignal> _showLoadingPublisher;

        [Inject]
        protected readonly IPublisher<ShowToastSignal> _showToastPublisher;

        [Inject]
        private readonly ISubscriber<GameActionSignal<IModuleContextModel>> _gameActionSubscriber;

        private DisposableBagBuilder _disposableBagBuilder;

        public Setting GameSetting => _gameSetting;

        private Action<GameAction, IModuleContextModel> Reducers;
        public ModelState GState { get; set; }
        private IScreenController _currentScreen;
        public ScreenName CurrentScreenName => _currentScreen.Name;

        private List<ModuleName> _lastHideModules = new();
        private ModuleName[] _hideModulesException = { ModuleName.Loading, ModuleName.Toast, ModuleName.Popup };

        public GameStore(
            Setting gameSetting,
            IObjectResolver container)
        {
            _gameSetting = gameSetting;
            _container = container;

            _definitionDataController = container.Resolve<IDefinitionDataController>();

            ReducerDelegating();
            GState = new();
        }

        public void Start()
        {
            _disposableBagBuilder = DisposableBag.CreateBuilder();
            _gameActionSubscriber.Subscribe(Dispatch).AddTo(_disposableBagBuilder);

            EnterNewScreen(ScreenName.SessionStart);
        }

        public void Dispose()
        {
            _disposableBagBuilder?.Build().Dispose();
        }

        public bool ChangeScreen(ScreenName newScreenName, bool isOutOldScreen = true)
        {
            if (newScreenName == _currentScreen.Name || !_currentScreen.IsAllowChangeScreen(newScreenName))
                return false;

            if (isOutOldScreen) _currentScreen.Out();
            EnterNewScreen(newScreenName);
            return true;
        }

        public bool ForceChangeScreen(ScreenName newScreenName)
        {
            _currentScreen.Out();
            EnterNewScreen(newScreenName);
            ScreenName previousScreenName = _currentScreen != null ? _currentScreen.Name : ScreenName.SessionStart;
            _gameScreenForceChangePublisher.Publish(new GameScreenForceChangeSignal(newScreenName, previousScreenName));
            return true;
        }

        public void ReEnterScreen()
        {
            EnterNewScreen(CurrentScreenName);
        }

        private void EnterNewScreen(ScreenName newScreenName)
        {
            ScreenName previousScreenName = _currentScreen != null ? _currentScreen.Name : ScreenName.SessionStart;
            _currentScreen = _container.Resolve<IReadOnlyList<IScreenController>>().ElementAt((int)newScreenName);
            _currentScreen.Enter();
            if (previousScreenName != newScreenName)
                _gameScreenChangePublisher.Publish(new GameScreenChangeSignal(newScreenName, previousScreenName));
        }

        #region Reducer pattern

        private void ReducerDelegating()
        {
            Reducers += TransitioningHandler;
        }

        private void Dispatch(GameActionSignal<IModuleContextModel> signal)
        {
            Reducers?.Invoke(signal.Action, signal.NewModel);
        }

        private void TransitioningHandler(GameAction action, IModuleContextModel model)
        {
            // test keep this structure to see in future if needed.
        }

        #endregion Reducer pattern

        #region Manipulate MVC pattern

        public void HideAllExcept(ModuleName[] modules)
        {
            foreach (var model in GState.Models.Values)
            {
                if (_hideModulesException.Contains(model.Module.ModuleName)) continue;

                if (modules != null && modules.Contains(model.Module.ModuleName) || !model.Module.ViewContext.View.activeInHierarchy) continue;
                model.Module.ViewContext.View.SetActive(false);
                _lastHideModules.Add(model.Module.ModuleName);
            }
        }

        public void RestoreLastHideModules()
        {
            foreach (var model in GState.Models.Values)
            {
                if (_hideModulesException.Contains(model.Module.ModuleName)) continue;

                if (_lastHideModules.Contains(model.Module.ModuleName))
                {
                    model.Module.ViewContext.View.SetActive(true);
                    _lastHideModules.Remove(model.Module.ModuleName);
                }
            }
        }

        public async UniTask<TClass> GetOrCreateModule<TClass, TModel>(
            string viewId,
            ViewName viewName,
            ModuleName moduleName)
            where TClass : IBaseModule
            where TModel : IModuleContextModel, new()
        {
            if (GState.HasModel<TModel>())
            {
                //_logger.Warning($"Dupplicate Found on Module: {typeof(TClass).ToString()}");
                return (TClass)GState.GetModel<TModel>().Module;
            }
            if (viewName == ViewName.Unity) viewId = moduleName.ToString() + "Script";
            TClass module = await CreateModuleInner<TClass, TModel>(moduleName);
            CreateModel<TClass, TModel>(module);
            BaseViewContext.Factory viewContextFactory = _container.Resolve<BaseViewContext.Factory>();
            IViewContext viewContext = viewContextFactory.Create(viewId, viewName);
            await module.CreateView(viewId, moduleName, viewContext);
            return module;
        }

        private async UniTask<TClass> CreateModuleInner<TClass, TModel>(ModuleName moduleName)
            where TClass : IBaseModule
            where TModel : IModuleContextModel, new()
        {
            BaseModule.Factory baseContextFactory = _container.Resolve<BaseModule.Factory>();
            TClass instance = (TClass)baseContextFactory.Create(moduleName);
            await instance.Initialize();
            return instance;
        }

        private void CreateModel<TClass, TModel>(TClass module)
            where TClass : IBaseModule
            where TModel : IModuleContextModel, new()
        {
            TModel model = GState.CreateNewModel<TModel>();
            model.Module = module;
        }

        public void RemoveAllModules()
        {
            GState.RemoveAllModules();
        }

        #endregion Manipulate MVC pattern

        #region Utils
        public bool CheckShowToastIfNotSuccessNetwork(GeneralResponse response)
        {
            if (!response.Success)
            {
                _showLoadingPublisher.Publish(new ShowLoadingSignal(isShow: false));
                _showToastPublisher.Publish(new ShowToastSignal(content: response.Message));
            }

            return !response.Success;
        }
        #endregion Utils
    }
}
