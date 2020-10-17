﻿using Microsoft.Extensions.DependencyInjection;
using Monica.AspectFlare.DependencyInjection;
using Monica.Enums;
using Monica.Module;

namespace Monica.Cache
{
    /// <summary>
    /// 添加缓存模块
    /// </summary>
    [DependsOnModules(typeof(AspectFlareModule))]
    public class CacheModule: StartupModule
    {
        public override ModuleLevel Level => ModuleLevel.FrameWork;
        public override uint Order => 1;

        public override void ConfigueServices(IServiceCollection services)
        {
            services.AddMonicaCacheMemory();
        }
    }
}
