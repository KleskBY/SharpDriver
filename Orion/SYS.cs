using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ClassByKamil
{
    public class SYS
    {
        public enum NTSTATUS : uint
        {
            Success = 0x00000000,   Wait0 = 0x00000000,  Wait1 = 0x00000001, Wait2 = 0x00000002,
            Wait3 = 0x00000003, Wait63 = 0x0000003f,   Abandoned = 0x00000080, AbandonedWait0 = 0x00000080,
            AbandonedWait1 = 0x00000081,  AbandonedWait2 = 0x00000082, AbandonedWait3 = 0x00000083,
            AbandonedWait63 = 0x000000bf, UserApc = 0x000000c0,  KernelApc = 0x00000100,
            Alerted = 0x00000101, Timeout = 0x00000102, Pending = 0x00000103,  Reparse = 0x00000104,
            MoreEntries = 0x00000105, NotAllAssigned = 0x00000106, SomeNotMapped = 0x00000107,
            OpLockBreakInProgress = 0x00000108, VolumeMounted = 0x00000109, RxActCommitted = 0x0000010a,
            NotifyCleanup = 0x0000010b,   NotifyEnumDir = 0x0000010c,  NoQuotasForAccount = 0x0000010d,
            PrimaryTransportConnectFailed = 0x0000010e, PageFaultTransition = 0x00000110, PageFaultDemandZero = 0x00000111,
            PageFaultCopyOnWrite = 0x00000112,  PageFaultGuardPage = 0x00000113,   PageFaultPagingFile = 0x00000114,
            CrashDump = 0x00000116, ReparseObject = 0x00000118,  NothingToTerminate = 0x00000122, ProcessNotInJob = 0x00000123,
            ProcessInJob = 0x00000124, ProcessCloned = 0x00000129, FileLockedWithOnlyReaders = 0x0000012a,FileLockedWithWriters = 0x0000012b,

            Informational = 0x40000000, ObjectNameExists = 0x40000000,   ThreadWasSuspended = 0x40000001,
            WorkingSetLimitRange = 0x40000002,  ImageNotAtBase = 0x40000003,   RegistryRecovered = 0x40000009,

            Warning = 0x80000000,   GuardPageViolation = 0x80000001, DatatypeMisalignment = 0x80000002,  Breakpoint = 0x80000003,
            SingleStep = 0x80000004, BufferOverflow = 0x80000005, NoMoreFiles = 0x80000006,
            HandlesClosed = 0x8000000a,  PartialCopy = 0x8000000d,  DeviceBusy = 0x80000011,
            InvalidEaName = 0x80000013,  EaListInconsistent = 0x80000014, NoMoreEntries = 0x8000001a,
            LongJump = 0x80000026,   DllMightBeInsecure = 0x8000002b,

            Error = 0xc0000000,   Unsuccessful = 0xc0000001, NotImplemented = 0xc0000002, InvalidInfoClass = 0xc0000003,
            InfoLengthMismatch = 0xc0000004, AccessViolation = 0xc0000005,InPageError = 0xc0000006, PagefileQuota = 0xc0000007,
            InvalidHandle = 0xc0000008,  BadInitialStack = 0xc0000009, BadInitialPc = 0xc000000a,
            InvalidCid = 0xc000000b,  TimerNotCanceled = 0xc000000c, InvalidParameter = 0xc000000d,  NoSuchDevice = 0xc000000e,
            NoSuchFile = 0xc000000f,  InvalidDeviceRequest = 0xc0000010, EndOfFile = 0xc0000011, WrongVolume = 0xc0000012,
            NoMediaInDevice = 0xc0000013, NoMemory = 0xc0000017, NotMappedView = 0xc0000019, UnableToFreeVm = 0xc000001a,
            UnableToDeleteSection = 0xc000001b, IllegalInstruction = 0xc000001d,AlreadyCommitted = 0xc0000021,
            AccessDenied = 0xc0000022, BufferTooSmall = 0xc0000023,ObjectTypeMismatch = 0xc0000024,
            NonContinuableException = 0xc0000025, BadStack = 0xc0000028,  NotLocked = 0xc000002a,
            NotCommitted = 0xc000002d,  InvalidParameterMix = 0xc0000030,  ObjectNameInvalid = 0xc0000033,
            ObjectNameNotFound = 0xc0000034,   ObjectNameCollision = 0xc0000035, ObjectPathInvalid = 0xc0000039,
            ObjectPathNotFound = 0xc000003a,ObjectPathSyntaxBad = 0xc000003b, DataOverrun = 0xc000003c,
            DataLate = 0xc000003d,  DataError = 0xc000003e,  CrcError = 0xc000003f,  SectionTooBig = 0xc0000040,
            PortConnectionRefused = 0xc0000041,  InvalidPortHandle = 0xc0000042, SharingViolation = 0xc0000043,
            QuotaExceeded = 0xc0000044, InvalidPageProtection = 0xc0000045,  MutantNotOwned = 0xc0000046,
            SemaphoreLimitExceeded = 0xc0000047, PortAlreadySet = 0xc0000048,SectionNotImage = 0xc0000049,
            SuspendCountExceeded = 0xc000004a,  ThreadIsTerminating = 0xc000004b, BadWorkingSetLimit = 0xc000004c,
            IncompatibleFileMap = 0xc000004d,   SectionProtection = 0xc000004e,   EasNotSupported = 0xc000004f,
            EaTooLarge = 0xc0000050,  NonExistentEaEntry = 0xc0000051, NoEasOnFile = 0xc0000052,
            EaCorruptError = 0xc0000053,  FileLockConflict = 0xc0000054, LockNotGranted = 0xc0000055,
            DeletePending = 0xc0000056,  CtlFileNotSupported = 0xc0000057, UnknownRevision = 0xc0000058, RevisionMismatch = 0xc0000059,
            InvalidOwner = 0xc000005a, InvalidPrimaryGroup = 0xc000005b, DeviceDoesNotExist = 0xc00000c0,
            UserExists = 0xc0000063, NoSuchUser = 0xc0000064,    GroupExists = 0xc0000065,NoSuchGroup = 0xc0000066,
            MemberInGroup = 0xc0000067,MemberNotInGroup = 0xc0000068,
            FileCheckedOut = 0xc0000901, CheckOutRequired = 0xc0000902,
            BadFileType = 0xc0000903, FileTooLarge = 0xc0000904, FormsAuthRequired = 0xc0000905,  VirusInfected = 0xc0000906,
            VirusDeleted = 0xc0000907, TransactionalConflict = 0xc0190001, InvalidTransaction = 0xc0190002,   TransactionNotActive = 0xc0190003, TmInitializationFailed = 0xc0190004,  RmNotActive = 0xc0190005,     RmMetadataCorrupt = 0xc0190006, TransactionNotJoined = 0xc0190007, DirectoryNotRm = 0xc0190008,  CouldNotResizeLog = 0xc0190009,
            TransactionsUnsupportedRemote = 0xc019000a,   LogResizeInvalidSize = 0xc019000b,  RemoteFileVersionMismatch = 0xc019000c,    CrmProtocolAlreadyExists = 0xc019000f,
            TransactionPropagationFailed = 0xc0190010,   CrmProtocolNotFound = 0xc0190011, TransactionSuperiorExists = 0xc0190012,
            TransactionRequestNotValid = 0xc0190013,TransactionNotRequested = 0xc0190014, TransactionAlreadyAborted = 0xc0190015,
            TransactionAlreadyCommitted = 0xc0190016, TransactionInvalidMarshallBuffer = 0xc0190017,
            CurrentTransactionNotValid = 0xc0190018, LogGrowthFailed = 0xc0190019,    ObjectNoLongerExists = 0xc0190021,
            StreamMiniversionNotFound = 0xc0190022, StreamMiniversionNotValid = 0xc0190023,
            MiniversionInaccessibleFromSpecifiedTransaction = 0xc0190024, CantOpenMiniversionWithModifyIntent = 0xc0190025,
            CantCreateMoreStreamMiniversions = 0xc0190026, HandleNoLongerValid = 0xc0190028,
            NoTxfMetadata = 0xc0190029, LogCorruptionDetected = 0xc0190030,  CantRecoverWithHandleOpen = 0xc0190031,
            RmDisconnected = 0xc0190032,  EnlistmentNotSuperior = 0xc0190033,
            RecoveryNotNeeded = 0xc0190034, RmAlreadyStarted = 0xc0190035,
            FileIdentityNotPersistent = 0xc0190036, CantBreakTransactionalDependency = 0xc0190037,
            CantCrossRmBoundary = 0xc0190038, TxfDirNotEmpty = 0xc0190039,
            IndoubtTransactionsExist = 0xc019003a, TmVolatile = 0xc019003b,
            RollbackTimerExpired = 0xc019003c, TxfAttributeCorrupt = 0xc019003d,
            EfsNotAllowedInTransaction = 0xc019003e, TransactionalOpenNotAllowed = 0xc019003f,
            TransactedMappingUnsupportedRemote = 0xc0190040, TxfMetadataAlreadyPresent = 0xc0190041,
            TransactionScopeCallbacksNotSet = 0xc0190042, TransactionRequiredPromotion = 0xc0190043,
            CannotExecuteFileInTransaction = 0xc0190044,  TransactionsNotFrozen = 0xc0190045,

            MaximumNtStatus = 0xffffffff
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING : IDisposable
        {
            public ushort Length;
            public ushort MaximumLength;
            private IntPtr buffer;

            public UNICODE_STRING(string s)
            {
                Length = (ushort)(s.Length * 2);
                MaximumLength = (ushort)(Length + 2);
                buffer = Marshal.StringToHGlobalUni(s);
            }

            public void Dispose()
            {
                Marshal.FreeHGlobal(buffer);
                buffer = IntPtr.Zero;
            }

            public override string ToString()
            {
                return Marshal.PtrToStringUni(buffer);
            }
        }

        [DllImport("ntdll.dll")]
        public static extern NTSTATUS RtlInitUnicodeString(ref UNICODE_STRING DestinationString,
             [MarshalAs(UnmanagedType.LPWStr)]String SourceString);

        public enum ADJUST_PRIVILEGE_TYPE
        {
            AdjustCurrentProcess,
            AdjustCurrentThread
        };

        public const int SeLoadDriverPrivilege = 10;
        public const int SeDebugPrivilege = 20;

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern NTSTATUS RtlAdjustPrivilege(int Privilege, bool Enable,
            ADJUST_PRIVILEGE_TYPE CurrentThread, out bool Enabled);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern NTSTATUS NtLoadDriver(ref UNICODE_STRING DriverServiceName);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern NTSTATUS NtUnloadDriver(ref UNICODE_STRING DriverServiceName);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(  string fileName, [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,   [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
            IntPtr securityAttributes,   [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,  [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr template);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint IoControlCode,   [MarshalAs(UnmanagedType.AsAny)][In] object InBuffer, uint nInBufferSize,
            [MarshalAs(UnmanagedType.AsAny)][Out] object OutBuffer, uint nOutBufferSize,   ref int pBytesReturned, IntPtr Overlapped);
    }
}
