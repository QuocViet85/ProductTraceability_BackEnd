use ProductTraceability;

GO

create procedure DM_tblDanhMuc_GetList
as
begin
	select *,
		(select * from tblDanhMuc as DMCon where DMCon.DM_DMCha_Id = DMCha.DM_Id for json path) as ListChildCategories
	from tblDanhMuc as DMCha where DMCha.DM_LaDMCha = 1 for json path;
end

GO