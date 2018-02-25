using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace PaketUpgrader
{
    public enum PaketUpgrade
    {
        UpgradeNeeded,
        NothingToDo
    }

    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();

            Console.ReadKey();
        }

        static async Task Run()
        {
            var token = Environment.GetEnvironmentVariable("GITHUB_ACCESS_TOKEN");
            var client = new GitHubClient(new ProductHeaderValue("paket-ugprade-scanner"))
            {
                Credentials = new Credentials(token)
            };

            var upgrader = new PaketUpgrader(client);

            var projects = new List<string>() {
                "fsprojects/FSharp.Configuration",
                 "15below/Ensconce",
                 "gigya/microdot",
                 "BlythMeister/Gallifrey",
                 "fsprojects/FSharpLint",
                 "jgrund/fable-jest",
                 "vik-borisov/TelegramClient",
                 "delegateas/XrmMockup",
                 "SAFE-Stack/SAFE-BookStore",
                 "fsprojects/Paket",
                 "CSBiology/BioFSharp",
                 "ionide/ionide-vscode-fsharp",
                 "fable-compiler/Fable",
                 "fsprojects/matprovider"
            };

            foreach (var project in projects)
            {
                await upgrader.Run(project);
            }

            //fwaris/CALib
            //Thorium/Owin.Compression
            //JohanLarsson/Gu.Reactive
            //red-gate/RedGate.Build
            //WebApiContrib/WebAPIContrib.Core
            //elastic/elasticsearch-net
            //mennowo/TLCGen
            //yurii-litvinov/REAL.NET
            //CSBiology/FSharp.Stats
            //janno-p/XRoadLib
            //krauthaufen/FShade
            //hafuu/FSharpApiSearch
            //SayToMe/Solve
            //gusty/FSharpPlus
            //fsdn-projects/FSDN
            //vbfox/stidgen
            //NinetailLabs/VaraniumSharp.Initiator
            //NinetailLabs/VaraniumSharp
            //punker76/code-samples
            //fslaborg/XPlot
            //ReedCopsey/Gjallarhorn
            //nhsevidence/ld-bnf
            //TheAngryByrd/MiniScaffold
            //KevM/tikaondotnet
            //fsprojects/FSharp.CloudAgent
            //fluentribbon/Fluent.Ribbon
            //aardvark-platform/aardvark.rendering
            //fsprojects/FSharp.TypeProviders.SDK
            //lefthandedgoat/canopyStarterKit
            //JohanLarsson/Gu.Wpf.DataGrid2D
            //mennowo/Gu.Wpf.DataGrid2D
            //stefanmaierhofer/Uncodium.Units
            //madhon/lighthouse
            //aabenoja/Cake.Parallel
            //MangelMaxime/Fulma
            //JohanLarsson/Gu.State
            //ionide/ionide-vscode-helpers
            //colinbull/colinbull.github.io
            //jet/kafunk
            //fsprojects/FSharp.AWS.DynamoDB
            //PaulStovell/ServiceBouncer
            //eiriktsarpalis/TypeShape
            //CSBiology/FSharp.Care
            //fsprojects/FSharp.Data.TypeProviders
            //enovales/LudumLinguarum
            //JasperFx/alba
            //fable-compiler/fable-react
            //nozzlegear/davenport.net
            //inosik/Fable
            //ncave/Fable
            //intellifactory/websharper
            //y2k/JoyReactorRN
            //steego/Steego.Demo
            //lefthandedgoat/canopy
            //nessos/Argu
            //fsprojects/Argu
            //NinetailLabs/NeuroLinker
            //JohanLarsson/Gu.Analyzers
            //pingthepeople/ptp-functions
            //jhoerr/iga-tracker-functions
            //intellifactory/websharper.ui.next
            //wastaz/Hyperboliq
            //muehlhaus/FSharp.Plotly
            //pbbwfc/Lizard
            //intellifactory/websharper.owin
            //intellifactory/websharper.forms
            //intellifactory/websharper.ui.next.piglets
            //intellifactory/websharper.html
            //sergey-tihon/OpenNLP.NET
            //nessos/FsPickler
            //mbraceproject/FsPickler
            //ionide/ionide-vscode-paket
            //fsharp/FsCheck
            //fscheck/FsCheck
            //OpenTl/OpenTl.Schema
            //johnberzy-bazinga/FSharp.Data.GraphQL
            //bazingatechnologies/FSharp.Data.GraphQL
            //fslaborg/FSharp.Charting
            //Antaris/RazorEngine
            //phatcher/CsvReader
            //haf/expecto
            //punker76/Fluent.Ribbon
            //fluentribbon/XamlCombine
            //eacasanovaspedre/functional-data-structures
            //fsprojects/IfSharp
            //fable-compiler/fable-suave-scaffold
            //draganjovanovic1/terminal-snake
            //intellifactory/websharper.o3d
            //intellifactory/websharper.leaflet
            //intellifactory/websharper.glmatrix
            //intellifactory/websharper.bing.maps
            //intellifactory/websharper.d3
            //intellifactory/websharper.charting
            //mvno/Okanshi
            //bjornna/dips-ckm
            //intellifactory/websharper.forms.bootstrap
            //F-2-F/F2F.ReactiveNavigation
            //phatcher/NCheck
            //kerams/Templatus
            //fsprojects/SwaggerProvider
            //Horusiath/Akkling
            //phatcher/SpecFlow.Unity
            //TIHan/Foom
            //fsharp/FsAutoComplete
            //baronfel/Paket
            //CSBiology/BioFSharp.Mz
            //getgauge/gauge-csharp
            //YaccConstructor/YaccConstructor
            //niklaslundberg/Arbor.X
            //gmpl/FSharpPlus
            //delegateas/XrmDefinitelyTyped
            //Zaid-Ajaj/LiteDB.FSharp
            //sergey-tihon/Stanford.NLP.NET
            //sqc1999/LxRunOffline
            //nozzlegear/alexa-skills
            //the-gamma/gallery-csv-service
            //the-gamma/thegamma-script
            //xiaoyvr/SimpleHttpMock
            //vrvis/aardvark.base
            //fsharp-editing/Forge
            //ai-traders/liget
            //fsprojects/FSharp.TypeProviders.StarterPack
            //RFQ-hub/SocketIoSuave
            //DigitalFlow/Xrm-Update-Context
            //fsprojects/FsXaml
            //readysetmark/WealthPulse
            //Hosch250/Checkers
            //isaksky/FsSqlDom
            //delegateas/Delegate.XrmDefinitelyTyped
            //JohanLarsson/Gu.Wpf.UiAutomation
            //JohanLarsson/Gu.Roslyn.Asserts
            //BillHally/FSharp.Chart
            //nessos/UnionArgParser
            //JohanLarsson/Gu.Wpf.ValidationScope
            //JohanLarsson/Gu.Wpf.ToolTips
            //JohanLarsson/Gu.Wpf.PropertyGrid
            //JohanLarsson/Gu.Wpf.NumericInput
            //JohanLarsson/Gu.Wpf.Media
            //JohanLarsson/Gu.Wpf.Geometry
            //JohanLarsson/Gu.Wpf.FlipView
            //JohanLarsson/Gu.Localization
            //JohanLarsson/Gu.Wpf.Adorners
            //fsprojects/FSharp.Control.AsyncSeq
            //chinwobble/DaisyParser
            //julienadam/AttachR
            //Thorium/Linq.Expression.Optimizer
            //dungpa/fantomas
            //mathnet/mathnet-symbolics
            //OrleansContrib/Orleankka
            //JohanLarsson/Gu.ChangeTracking
            //JohanLarsson/Gu.Persist
            //JohanLarsson/Gu.Settings
            //DotNetAnalyzers/WpfAnalyzers
            //vrvis/aardvark
            //Creuna-Oslo/Episerver.Basis.Slim
            //janno-p/XRoadProvider
            //tpetricek/FSharp.Formatting
            //jackfoxy/FSharp.Formatting
            //wikibus/Argolis
            //daniel-chambers/FSharp.Azure.Storage
            //fsprojects/FSharp.Azure
            //fsprojects/FSharp.Azure.Storage
            //fsprojects/ProjectScaffold
            //AmyrisInc/GslCore
            //cloudRoutine/Paket
            //tytusse/GitVersionTypeProvider
            //mennowo/MiniTD
            //cloudRoutine/FSharp.Formatting
            //AlaricDelany/Schwefel.Ruthenium
            //BayardRock/IfSharp
            //persimmon-projects/Persimmon
            //pdfforge/translatable
            //fsprojects/Paket.VisualStudio
            //vbfox/Paket
            //questnet/FSCS-Lensgenerator
            //embix/FSCS-Lensgenerator
            //RMCKirby/ColorConsole
            //dsyme/FSharp.TypeProviders.StarterPack
            //JohanLarsson/Gu.Inject
            //aardvarkplatform/aardvark.docs
            //kichristensen/InfluxDB.WriteOnly
            //mastoj/kalender2015
            //teo-tsirpanis/FsRandom-pcg
            //EveResearchFoundation/EveGameEngine
            //callmekohei/deopletefs
            //tpluscode/UriTemplateString
            //pocketberserker/Data.HList
            //TheAngryByrd/Marten.FSharp
            //vrvis/aardvark.rendering
            //erbg/HRON
            //persimmon-projects/Persimmon.MuscleAssert
            //sergey-tihon/FSharp.TypeProviders.StarterPack
            //fjoppe/Legivel
            //fjoppe/FsYamlParser
            //JohanLarsson/Gu.Units
            //muehlhaus/BioFSharp
            //hoonzis/Pricer
            //persimmon-projects/Persimmon.Script
            //jorgef/fsharpworkshop
            //madhon/statsd.net
            //madhon/ostrich
            //rflechner/Suave.Swagger
            //sideeffffect/FSharpx.Collections
            //95ulisse/FsCPS
            //vreniose95/Ccr
            //reflash/chat-backend
            //VasiliskDevelopment/CardboardFactorySolution
            //smolyakoff/conreign
            //sergey-tihon/SwaggerProvider
            //mattstermiller/koffee
            //giacomociti/FSharp.Data.Xsd
            //unic/bob-keith
            //jkone27/AliceMQ
            //demystifyfp/FsTweet
            //sillyotter/BackupDBToAzure
            //theimowski/SuaveMusicStore
            //YaccConstructor/Brahma.FSharp
            //fsprojects/AzureStorageTypeProvider
            //arthis/glomulish
            //rflechner/LinqToSalesforce
            //0x53A/Paket
            //thinkbeforecoding/Paket
            //dady8889/Fluent.Ribbon
            //RFQ-hub/ZabbixAgentLib
            //janno-p/fable-vue
            //whitetigle/samples-pixi
            //mbraceproject/MBrace.Azure
            //AmyrisInc/Gslc
            //WalternativE/fableshotdetection
            //ionide/ionide-fsgrammar
            //hanswolff/dedilib
            //cloudRoutine/FAKE
            //jjpatel/pushtray
            //mbraceproject/MBrace.Core
            //teo-tsirpanis/Farkle
            //ivaylo5ev/mason
            //YaccConstructor/Examples
            //e-rik/XRoadLib
            //phatcher/Meerkat.Security.Hmac
            //rflechner/FSharp.Data
            //codehutch/pg3d
            //nu-soft/FSharp.Plc.Ads
            //CaringDev/FsUnit.xUnit.Typed
            //boumenot/Wapiti.NET
            //Jallah/Connect4Challenge
            //HLWeil/BioFSharp
            //fsprojects/Canopy.Mobile
            //ed-ilyin/FSharp.EdIlyin.Core
            //mbraceproject/MBrace.AWS
            //also-cloud/AlsoCloud.Marketplace
            //VaseninaAnna/Brahma.FSharp
            //danieljsummers/FromObjectsToFunctions
            //pkese/Orleankka
            //ShalamovRoman/YC.Web
            //bvenn/BioFSharp
            //editorconfig/editorconfig-visualstudio
            //FSharp-CN/FSharpBB
            //baronfel/FAKE
            //migfig/Labs
            //ExM/NMoney
            //ar3cka/Journalist
            //mbraceproject/Vagabond
            //nessos/Vagabond
            //ExM/NConfiguration
            //cgravill/TypesTSFS
            //delegateas/Delegate.XrmContext
            //nessos/Thespian
            //ncave/fable-repl
            //steego/toychest
            //datNET/fs-fake-targets
            //DigitalFlow/Xrm-Oss-Interfacing
            //corux/seafile-cli-windows
            //popular-parallel-programming/quad-ropes
            //pocketberserker/FSDN
            //fable-compiler/samples
            //vbfox/FAKE
            //MiloszKrajewski/FAKE
            //yodiz/HttpApp
            //ploeh/FsCheck
            //MatrixSolutions/FSharp.Data.JsonValidation
            //jeremyabbott/Presentations
            //lefthandedgoat/openfsharpdemo
            //pblasucci/quickpbt
            //mleech/scotch
            //GraanJonlo/MiniScaffold
            //Dzoukr/Fue
            //mhertis/Orleankka
            //UgurAldanmaz/ServiceBouncer
            //Lleutch/Sast
            //leleutch/GenerativeTypeProviderExample
            //amazingant/Amazingant.FSharp.TypeExpansion.Templates
            //NinetailLabs/VaraniumSharp.Monolith
            //LambdaFactory/fable-vscode-demo
            //nhsevidence/ld-viewer
            //nhsevidence/ld-qs-viewer
            //sideeffffect/FSharpx.Extras
            //elic-io/Hello.World
            //fsprojects/FSharp.Data.Toolbox
            //vrvis/fablish
            //OlegZee/Xake
            //cagyirey/Capstone.FSharp
            //TheAngryByrd/Fable.Template.Library
            //ElijahReva/ticket-problem
            //awright18/SimpleExcelExporter
            //sergey-tihon/Paket
            //Cubey2019/Fluent.Ribbon
            //teo-tsirpanis/expecto
            //kMutagene/BioFSharp
            //artur-s/Paket
            //matthid/Yaaf.FSharp.Scripting
            //haraldsteinlechner/FsPickler
            //barsgroup/linq2db
            //barsgroup/bars2db
            //Spreads/Spreads.R
            //lukasimir/SimJobShop1
            //jeroldhaas/Gjallarhorn
            //theprash/AzureStorageTypeProvider
            //varon/FSLogger
            //TOTBWF/FSharp.Data.GraphQL
            //hodzanassredin/NFastText
            //fsprojects/ExcelProvider
            //CSBiology/FSharp.FGL
            //irustandi/ReinforcementLearningFSharp
            //BayardRock/Fungible
            //blair55/canopy
            //TeaDrivenDev/Amagatsha
            //TeaDrivenDev/BranchDocuments
            //cloudRoutine/FsAutoComplete
            //nicolocodev/suavegnalr
            //DingpingZhang/MaterialDesignInXamlToolkit
            //TahaHachana/XPlot
            //relayfoods/FsToolkit
            //MattiasJakobsson/SuperGlue
            //SuperGlueFx/SuperGlue
            //fsprojects/projekt
            //johnazariah/roslyn-wrapper
            //andburn/hs-art-extractor
            //mattj23/mathnet-spatial
            //ArtofQuality/F2F.ReactiveNavigation
            //sideeffffect/ProjectScaffold
            //aolney/braintrust-server
            //robertmuehsig/Fluent.Ribbon
            //lexarchik/Paket
            //Amyris/AmyrisBio
            //Rickasaurus/Barb
            //toburger/FSharp.Json
            //divad4686/places-google-details
            //15below/Sproc.Lock
            //andredublin/Forge
            //cryosharp/fluentwindsor
            //JohnStov/Frobnicator2
            //hoonzis/fluentnest
            //CSGOpenSource/elasticsearch-net
            //NUIM-BCL/DiffSharp
            //DiffSharp/DiffSharp
            //gilles-leblanc/genesis
            //tjaskula/akka.net-fsharp.extensions
            //ploeh/PollingConsumer
            //Steinpilz/confifu-commands
            //pittsw/TheWanderer
            //eriove/mathnet-numerics
            //ChipmunkHand/GiraffeRescue
            //wooga/Paket.Unity3D
            //ed-ilyin/EdIlyin.FSharp.Elm.Core
            //daz10000/fss
            //pkanavos/FakeCake_DNZ
            //fsprojects/ExcelFinancialFunctions
            //vilinski/MongoDB.Bson.FSharp
            //pirrmann/FSketch
            //johnazariah/csharp-algebraictypes
            //johnazariah/csharp-uniontypes
            //fsprojects/FSharp.Management
            //RinsibleElk/TextOn.Atom
            //contactsamie/FinCa
            //miklund/inquiry
            //Stift/Paket
            //CSBiology/FSharpGephiStreamer
            //fslaborg/RProvider
            //fsprojects/FSharp.Compiler.CodeDom
            //fsprojects/FSharpx.Collections
            //ksmirenko/Brahma.FSharp
            //TeaDrivenDev/StatusPotentiae
            //fsharp-editing/fstoml
            //fsprojects/FsLexYacc
            //Hopac/Hopac
            //cotyar/Akkling
            //johanclasson/Unity.Interception.Serilog
            //npmurphy/IfSharp
            //scottgrey/weather
            //the-gamma/govuk-data-import
            //spr13/ACWES
            //rneatherway/FsAutoComplete
            //bullmastiffo/Contour
            //josselinauguste/exim-maillog
            //mandest/FSFabric
            //mseknibilel/Alfred
            //dsyme/MBrace.Azure
            //dsyme/MBrace.Core
            //adelnizamutdinov/music-page
            //dsyme/FSharp.Charting
            //madhon/Formo
            //tipunch74/MaterialDesignInXamlToolkit
            //eerohele/oath
            //TheInnerLight/Nomad
            //sfukui/MNNPlus
            //sideeffffect/FinFun.Contracts
            //boloutaredoubeni/kaleidoscope
            //mhtraylor/market-sim
            //Slesa/Dotter
            //dkholod/GoogleCloudChaosMonkey
            //cagyirey/Shodan.FSharp
            //mastoj/BigQueryProvider
            //caindy/ODataTypeProvider
            //csjune/mathnet-spatial
            //mathnet/mathnet-filtering
            //skalinets/eat-now
            //mandest/Nap
            //colinbull/SqlProvider
            //VerbalExpressions/FSharpVerbalExpressions
            //enovales/feltlion
            //kellerd/TicTacToeProvider
            //stewart-r/AzureDocumentDbTypeProvider
            //pkanavos/CSharpRevolution_DNZ
            //mvno/Franz
            //TheInnerLight/CSharp.Formatting
            //Thilas/commandline
            //pittsw/Indy.NET
            //persimmon-projects/Persimmon.Unquote
            //giacomociti/AntaniXml
            //teo-tsirpanis/brainsharp
            //sergey-tihon/FSharp.Management
            //Steinpilz/confifu
            //sergey-tihon/FSharp.Compiler.Service
            //fsharp/xamarin-monodevelop-fsharp-addin
            //danm-de/Fractions
            //beeker/FAKE
            //mavnn/Taggen
            //FsStorm/FsStorm
            //bennylynch/SQLProvider
            //tgrospic/advent-of-code
            //mzabolotko/Contour
            //SDVentures/Contour
            //Badmoonz/FSharp.ReportDSL
            //fsharp-editing/FSharp.Editing
            //FSharpBristol/FSharpBristolPresentations
            //jmquigs/ModelMod
            //cloudRoutine/toml-fs
            //matthid/Yaaf.AdvancedBuilding
            //carbon-twelve/Akabeko
            //CumpsD/knx.net
            //sidburn/sidburn.github.io
            //vossccp/BotBuilderChannelConnector
            //0x53A/FSharp.Compiler.Service
            //snowcrazed/Paket
            //figrollapps/PersonalTrainer
            //pocketberserker/FSharp.Object.Diff
            //robertpi/caffe.clr.demo
            //rackspace/rackspace-net-sdk
            //alfonsogarciacaro/FSharp.Compiler.Service
            //forki/FSharp.Compiler.Service
            //xavierzwirtz/FSharp.QueryProvider
            //voiceofwisdom/FSharp.QueryProvider
            //zeromq/fszmq
            //pblasucci/fszmq
            //jbeeko/TypeUp
            //object/MidiFsharp
            //cosmo0920/grnline.fs
            //rfrerebe/fable-suave-scaffold
            //adam-mccoy/elasticsearch-net
            //Thorium/Paket
            //rflechner/SlackTypeProvider
            //pocketberserker/ZeroFormatter.FSharpExtensions
            //simonhdickson/Paket
            //soerennielsen/canopy
            //imazen/libwebp-net
            //tpetricek/Deedle
            //czifro-tech/30-days-of-fsharp
            //AITGmbH/ApplyCompanyPolicy.Template
            //AITGmbH/AIT-Apply-Company-Policy-SDK
            //dolly22/FAKE.Dotnet
            //alexeyzimarev/MassTransit.RavenDbIntegration
            //et1975/fable-suave-scaffold
            //mjul/aws-lambda-fsharp-lab
            //wallymathieu/SemVer.FromAssembly
            //mvkra/WebsocketFrameworkTester
            //vain0/EnumerableTest
            //ScottShingler/FsStorm
            //dangets/ionide-atom-fsharp
            //ionide/ionide-atom-fsharp
            //ionide/ionide-fsharp
            //fbehrens/vscode-polyglott
            //fsharping/Website
            //msdyncrm-contrib/PluginAssemblyLoader
            //ionide/ionide-vscode-fake
            //jeremyabbott/doctoralsurvey
            //ijsgaus/fun
            //rexcfnghk/marksix-parser
            //rojepp/FSharp.Compiler.Service
            //fradav/IfSharp
            //deapsquatter/FSharp.SSEClient
            //alexpantyukhin/FSharp.Data.GraphQL
            //fsprojects/FSharp.ViewModule
            //pocketberserker/ComVu
            //mjul/dropbox-activity-reports
            //TIHan/Ferop
            //opcon/turnt-ninja
            //inosik/Paket
            //arthis/psychic-tribble
            //davidpodhola/fable-suave-scaffold
            //amazingant/Amazingant.FSharp.TypeExpansion
            //TomGillen/GoldenHammer
            //TheInnerLight/FSPipes
            //bchavez/Dwolla
            //ksmirenko/type-providers
            //jindraivanek/fueBlog
            //HackYourJob/hexagonchallenge
            //Graph-Parsing-Demo-Group/YC.GraphParsingDemo
            //Bio-graph-group/YC.BioGraph
            //mjul/fslab-playground
            //KochetovKirill/YC.GraphParsingDemo
            //ZachBray/FunScript
            //vbfox/U2FExperiments
            //kate-paulk/tae_exam_candidate_kp
            //statmuse/TokensRegexProvider
            //kuroyakov/fsharp-study
            //wikibus/Rdf.Vocabularies
            //eugene-g/ambiata-weather
            //rexcfnghk/FSharp.Backup
            //cloudRoutine/VisualFSharpPowerTools
            //fsprojects/VisualFSharpPowerTools
            //kjnilsson/FSharp.AutoComplete
            //ionide/ionide-fsi
            //ionide/ionide-helpers
            //jmjrawlings/SpannerFS
            //BlythMeister/BingImageDowload
            //frankshearar/etwas
            //Giusy151/mathnet-spatial
            //xdaDaveShaw/FSharp-Suave-Expecto
            //lefthandedgoat/genit
            //Bomret/Unit
            //cloudRoutine/Forge
            //isaacabraham/Paket
            //MnZrK/fundcpp
            //MnZrK/fdcpp
            //fsprojects/FSharpx.Extras
            //Bomret/NeverNull
            //theor/GitShow
            //smkell/fsfbl
            //the-gamma/thegamma-olympics-web
            //DIPSASA/dips-ckm
            //fsprojects/FSharpx.Async
            //NinetailLabs/Cake.PaketRestore
            //mathias-brandewinder/wonderland-fsharp-katas
            //endeavour/PerfectShuffle.WebsharperExtensions
            //hsharpsoftware/use-case-maker
            //pmbanka/kkm-reminder
            //hmemcpy/Paket.VisualStudio
            //enovales/FsGettextUtils
            //bartelink/FunDomain
            //john-patterson/FSharp.Desktop.UI
            //wikibus/JsonLD.Entities
            //persimmon-projects/Persimmon.Dried
            //sylvanc/CamdenTown
            //TheAngryByrd/OWASPPipelineSlides
            //SDVentures/HostBox
            //mfwilson/myriad
            //larzw/Cake.Paket.Example
            //OlegZee/Suave.OAuth
            //endeavour/PerfectShuffle.Security
            //nhsevidence/ld-compiler
            //blainne/RightHandOfFate
            //gjuljo/HelloSuave
            //willryan/MemoryWaybackAPI
            //FoothillSolutions/FunctionalEconnect
            //enerqi/AlgorithmPad
            //JeremyBellows/NeuralFish"


        }
    }

    public class PaketUpgrader
    {
        private GitHubClient client;

        private string updatedPaketSha = "b98e000b232408fe0a21730e88f89755f0d7a68c";

        public PaketUpgrader(GitHubClient client)
        {
            this.client = client;
        }

        public async Task Run(string ownerAndName, bool performUpgrade = false)
        {
            var parts = ownerAndName.Split('/');
            var owner = parts[0];
            var name = parts[1];

            var result = await Validate(owner, name);

            if (result == PaketUpgrade.NothingToDo)
            {
                Console.WriteLine($"Nothing to do for {ownerAndName}");
                return;
            }

            await PerformUpgrade(owner, name, performUpgrade);
        }

        private async Task<PaketUpgrade> Validate(string owner, string name)
        {
            var repository = await client.Repository.Get(owner, name);
            var defaultBranch = repository.DefaultBranch;

            try
            {
                var contents = await client.Repository.Content.GetAllContentsByRef(owner, name, ".paket", defaultBranch);
                var executables = contents.Where(c => c.Path.EndsWith(".exe"));
                foreach (var executable in executables)
                {
                    if (executable.Path.EndsWith("paket.exe") && executable.Sha != updatedPaketSha)
                    {
                        return PaketUpgrade.UpgradeNeeded;
                    }

                    if (executable.Path.EndsWith("paket.bootstrapper.exe") && executable.Sha != updatedPaketSha)
                    {
                        return PaketUpgrade.UpgradeNeeded;
                    }
                }
            }
            catch { }


            return PaketUpgrade.NothingToDo;
        }

        async Task PerformUpgrade(string owner, string name, bool performUpgrade)
        {
            var openPullRequest = await HasOpenPullRequest(owner, name);
            if (openPullRequest != null)
            {
                Console.WriteLine($"{owner}/{name} has an open pull request #{openPullRequest.Number}, skipping...");
            }
            else
            {
                Console.WriteLine($"No open pull request found, gotta upgrade {owner}/{name}");

                if (!performUpgrade)
                {
                    Console.WriteLine($"Skipping actual upgrade step for now");
                    return;
                }

                var repository = await FindRepositoryToSubmitPullRequestFrom(owner, name);
                if (repository == null)
                {
                    Console.WriteLine($"Couldn't find a repository to use for upgrading. WTF?");
                    return;
                }

                var reference = await CreateNewReferenceWithPatch(repository);
                if (reference != null)
                {
                    var branch = reference.Ref.Replace("refs/heads/", "");

                    var headRef = repository.Fork ? $"{repository.Owner.Login}:{branch}" : branch;

                    var newPullRequest = new NewPullRequest("Update paket to address TLS deprecation", headRef, repository.DefaultBranch);
                    newPullRequest.Body = @":wave: GitHub disabled TLS 1.0 and TLS 1.1 on February 22nd, which affected Paket.

You can read more about this on the [GitHub Engineering blog](https://githubengineering.com/crypto-removal-notice/).

The update to Paket is explained here: https://github.com/fsprojects/Paket/pull/3066

The work to update Paket in the wild is occurring here: https://github.com/fsprojects/Paket/issues/3068";

                    var pullRequest = await client.PullRequest.Create(owner, name, newPullRequest);
                }
            }
        }

        async Task<PullRequest> HasOpenPullRequest(string owner, string name)
        {
            var pullRequests = await client.PullRequest.GetAllForRepository(owner, name, new ApiOptions() { PageSize = 100 });

            foreach (var pullRequest in pullRequests)
            {
                var files = await client.PullRequest.Files(owner, name, pullRequest.Number);
                var updatesPaketToLatestVersion = files.FirstOrDefault(f =>
                    f.FileName == ".paket/paket.exe" && f.Sha == updatedPaketSha || f.FileName == ".paket/paket.bootstrapper.exe" && f.Sha == updatedPaketSha);

                if (updatesKeyFiles != null)
                {
                    return pullRequest;
                }
            }

            return null;
        }

        async Task<Repository> FindRepositoryToSubmitPullRequestFrom(string owner, string name)
        {
            var repository = await client.Repository.Get(owner, name);
            if (repository.Permissions.Push)
            {
                return repository;
            }

            var request = new RepositoryRequest()
            {
                Type = RepositoryType.Owner
            };

            var ownedRepos = await client.Repository.GetAllForCurrent(request, new ApiOptions() { PageSize = 100 });
            var forks = ownedRepos.Where(r => r.Fork);
            var matchingNames = forks.Where(r => r.Name == name);
            var foundFork = matchingNames.FirstOrDefault();

            // TODO: foundFork.Parent is null, and that might be a problem down the track

            if (foundFork != null)
            {
                return foundFork;
            }

            return await client.Repository.Forks.Create(owner, name, new NewRepositoryFork());
        }

        async Task<string> GetNewExecutableBase64()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            var resourceName = resourceNames[0];

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        async Task<Reference> CreateNewReferenceWithPatch(Repository repository)
        {
            // first, create a new blob in the repository which is the new file contents
            var blob = new NewBlob()
            {
                Content = await GetNewExecutableBase64(),
                Encoding = EncodingType.Base64
            };
            var newBlob = await client.Git.Blob.Create(repository.Id, blob);

            // we create the new reference for the PR branch
            var defaultRef = $"heads/{repository.DefaultBranch}";
            var defaultBranch = await client.Git.Reference.Get(repository.Id, defaultRef);
            var initialSha = defaultBranch.Object.Sha;

            var newRef = $"heads/bootstrapper";
            var newReference = await client.Git.Reference.Create(repository.Id, new NewReference(newRef, initialSha));

            var currentTree = await client.Git.Tree.Get(repository.Id, initialSha);

            // update the paket subdirectory to assign the new blob to whatever executable
            var paketTreeNode = currentTree.Tree.FirstOrDefault(t => t.Path == ".paket");
            var paketTree = await client.Git.Tree.Get(repository.Id, paketTreeNode.Sha);

            var executables = paketTree.Tree.Where(t => t.Path.EndsWith(".exe"));

            if (executables.Count() == 0)
            {
                Console.WriteLine($"TODO: oh gosh, we're not able to find executables in the .paket directory");
                return null;
            }

            var executable = executables.ElementAt(0);

            var newPaketTree = new NewTree
            {
                BaseTree = paketTree.Sha,
            };
            newPaketTree.Tree.Add(new NewTreeItem()
            {
                Mode = executable.Mode,
                Sha = newBlob.Sha,
                Path = executable.Path,
                Type = executable.Type.Value
            });

            var updatedPaketTree = await client.Git.Tree.Create(repository.Id, newPaketTree);

            // update the root tree to use this new .paket directory
            var newRootTree = new NewTree
            {
                BaseTree = currentTree.Sha
            };
            newRootTree.Tree.Add(new NewTreeItem()
            {
                Mode = paketTreeNode.Mode,
                Sha = updatedPaketTree.Sha,
                Path = paketTreeNode.Path,
                Type = paketTreeNode.Type.Value
            });

            var updatedRootTree = await client.Git.Tree.Create(repository.Id, newRootTree);

            // create a new commit using the updated tree
            var newCommit = new NewCommit($"Updated {executable.Path} to address TLS 1.0 and 1.1 deprecation", updatedRootTree.Sha, initialSha);
            var commit = await client.Git.Commit.Create(repository.Id, newCommit);

            // then update the bootstrapper ref to this new commit
            var updatedReference = await client.Git.Reference.Update(repository.Id, newRef, new ReferenceUpdate(commit.Sha));

            return updatedReference;
        }
    }
}
