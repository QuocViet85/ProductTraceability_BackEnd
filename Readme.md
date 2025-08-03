Logic phân quyền tổng quát:
- Admin có quyền thao tác toàn diện tuyệt đối.
- User không phải Admin chỉ nên thao tác với tài nguyên User sở hữu (bao gồm sở hữu trực tiếp và sở hữu gián tiếp qua tài nguyên khác), nếu có thao tác với tài nguyên của User khác thì nên rất hạn chế, chỉ áp dụng trong trường hợp bắt buộc phải có.

Các tài nguyên có thực sẽ thuộc về hộ kinh doanh cá nhân (Individual Enterprise) hoặc doanh nghiệp (Enterprise):
- Có thể kiểm tra tài nguyên của hộ kinh doanh cá nhân của User nào bằng UserId vì hộ kinh doanh cá nhân của User có Id = UserId

Lấy tài nguyêm:
- api/ hàm repo, service lấy trực tiếp tài nguyên nào nằm ở area(module) của tài nguyên đó.
