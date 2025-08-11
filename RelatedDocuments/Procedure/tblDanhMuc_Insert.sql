use ProductTraceability;

GO

create procedure DM_tblDanhMuc_Insert
@name nvarchar(500),
@moTa nvarchar(max),
@nguoiTao_Id uniqueidentifier,
@laDMCha bit,
@dMChaId_Id uniqueidentifier
as
begin
	if @laDMCha = 1
		begin
			if @dMChaId_Id IS NOT NULL
				begin
					raiserror('Danh mục cha không thể có danh mục cha', 16, 1);
					return;
				end
		end

	if @laDMCha = 0
		begin
			if @dMChaId_Id IS NULL
				begin
					raiserror('Danh mục con không thể không có danh mục cha', 16, 1);
					return;
				end
		end

	insert into [tblDanhMuc] (DM_Name, DM_MoTa, DM_NguoiTao_Id, DM_LaDMCha, DM_DMCha_Id)
		values (@name, @moTa, @nguoiTao_Id, @laDMCha, @dMChaId_Id);
end

GO
	
	
