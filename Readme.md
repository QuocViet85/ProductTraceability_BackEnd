Lấy tài nguyêm:
- api/ hàm repo, service lấy trực tiếp tài nguyên nào nằm ở area(module) của tài nguyên đó.

Phân quyền
- Role Admin có toàn quyền hệ thống, muốn làm gì thì làm.
- Role Enterprise:

* Chỉ được thao tác theo QUYỀN (Claim) với các Tài nguyên liên kết với tài khoản.

Các trường trong Model:
- Trường do BackEnd gán: luôn nullable để client không gửi lên thì không lỗi.
- Trường nếu Client không gửi thì BackEnd tự động gán (Điển hình là mã định danh ngoài ID): luôn nullable để client không gửi lên thì không lỗi. Khi update, client không gửi thì để nguyên giá trị cũ.
- Trường do Client gán: nullale hay không theo thiết kế database.
