namespace ZK.Data.BLL.MICardReader
{
	public enum ReadCardErrorType
	{
		NewKeyStringLengthError = -106,
		NotFirstBlockInSector,
		WritingKeyBlock,
		OpBlockCountOutOfRange,
		KeyDataLengthError,
		SectorIdOutOfRange,
		BlockIdOutOfRange,
		BlockCountTooLong = -10,
		Succeed = 0,
		Failed,
		NoReaderDetected = 5,
		SetParameterFailed = 129,
		Timeout,
		CardNotExistsOrAuthFailed,
		ReceiveDataError,
		CommandFormatError,
		UnknownError = 135,
		WrongCardKey = 140,
		CommandNotExists = 143
	}
}
