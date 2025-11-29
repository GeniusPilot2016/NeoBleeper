# 🤝 Đóng góp cho NeoBleeper

Trước hết, xin cảm ơn bạn đã cân nhắc đóng góp cho NeoBleeper! Những đóng góp của bạn là một phần không thể thiếu cho sự thành công của dự án này. Cho dù bạn đang báo cáo lỗi, đề xuất tính năng, cải thiện tài liệu, tải lên tệp BMM hoặc tệp NBPML cũ, hay gửi mã, sự tham gia của bạn đều rất được trân trọng.

## 📑 Mục lục
1. [Quy tắc ứng xử](#-quy-t%E1%BA%AFc-%E1%BB%A9ng-x%E1%BB%AD)
2. [Tôi có thể đóng góp bằng cách nào?](#%E2%80%8D%EF%B8%8F-t%C3%B4i-c%C3%B3-th%E1%BB%83-%C4%91%C3%B3ng-g%C3%B3p-nh%C6%B0-th%E1%BA%BF-n%C3%A0o)
    - [Báo cáo lỗi](#-b%C3%A1o-c%C3%A1o-l%E1%BB%97i)
    - [Yêu cầu tính năng](#-y%C3%AAu-c%E1%BA%A7u-t%C3%ADnh-n%C4%83ng)
    - [Đóng góp mã](#%E2%80%8D-%C4%91%C3%B3ng-g%C3%B3p-m%C3%A3)
    - [Tài liệu](#-t%C3%A0i-li%E1%BB%87u)
    - [Đóng góp tệp BMM và NBPML](#-%C4%91%C3%B3ng-g%C3%B3p-t%E1%BB%87p-bmm-v%C3%A0-nbpml)
3. [Quy trình yêu cầu kéo](#%EF%B8%8F-quy-tr%C3%ACnh-y%C3%AAu-c%E1%BA%A7u-k%C3%A9o)
4. [Hướng dẫn kiểu](#-h%C6%B0%E1%BB%9Bng-d%E1%BA%ABn-phong-c%C3%A1ch)
    - [Kiểu mã](#-phong-c%C3%A1ch-m%C3%A3)
    - [Ghi chú cụ thể về C#](#-ghi-ch%C3%BA-c%E1%BB%A5-th%E1%BB%83-v%E1%BB%81-c)
5. [Hỗ trợ cộng đồng](#%E2%80%8D%E2%80%8D%E2%80%8D-h%E1%BB%97-tr%E1%BB%A3-c%E1%BB%99ng-%C4%91%E1%BB%93ng)

## 🌟 Quy tắc ứng xử
Bằng cách tham gia dự án này, bạn Đồng ý tuân thủ Quy tắc Ứng xử. Vui lòng tôn trọng và quan tâm đến những người khác trong cộng đồng. Xem tệp `CODE_OF_CONDUCT-vn.md` để biết chi tiết.

## 🤝🙋‍♂️ Tôi có thể đóng góp như thế nào?

### 🪲 Báo cáo Lỗi
Nếu bạn tìm thấy lỗi trong NeoBleeper, vui lòng tạo một vấn đề và bao gồm các thông tin sau:
  - Tiêu đề rõ ràng và mô tả chi tiết.
  - Phiên bản NeoBleeper hoặc mã băm (nếu có).
  - Các bước để tái hiện vấn đề hoặc một đoạn mã.
  - Hành vi dự kiến ​​và thực tế.
  - Bất kỳ thông tin liên quan nào khác, bao gồm ảnh chụp màn hình hoặc nhật ký sự cố.

### 💭 Yêu cầu Tính năng
Chúng tôi hoan nghênh ý tưởng của bạn! Để yêu cầu một tính năng:
1. Kiểm tra các vấn đề để xem đã có người khác yêu cầu chưa.
2. Nếu chưa, hãy mở một vấn đề mới và chia sẻ mô tả chi tiết bao gồm:
  - Bối cảnh của yêu cầu.
  - Tại sao yêu cầu này lại có giá trị.
  - Tác động tiềm ẩn, rủi ro hoặc cân nhắc.

### 👩‍💻 Đóng góp Mã
1. Fork kho lưu trữ và tạo một nhánh mới ngoài `main`. Đặt tên nhánh dễ hiểu, chẳng hạn như `feature/add-tune-filter`.
2. Mở thư mục kho lưu trữ trong Visual Studio:
    - Đảm bảo bạn đã cài đặt [Visual Studio](https://visualstudio.microsoft.com/) với các khối lượng công việc cần thiết (ví dụ: ".NET desktop development" cho NeoBleeper).
    - Sao chép nhánh kho lưu trữ của bạn vào máy cục bộ (bạn có thể sử dụng các công cụ Git tích hợp của Visual Studio hoặc Git CLI).
    - Sau khi sao chép, hãy mở tệp giải pháp (`.sln`) trong Visual Studio.
3. Cài đặt Gói NuGet:
    - Khôi phục bất kỳ phần phụ thuộc nào cần thiết bằng cách nhấp vào `Restore NuGet Packages` ở thanh trên cùng hoặc chạy `dotnet restore` từ terminal.
4. Thêm các thay đổi của bạn:
    - Sử dụng các tính năng của Visual Studio như IntelliSense, gỡ lỗi và định dạng mã để đóng góp hiệu quả.
    - Đảm bảo các bài kiểm tra phù hợp được bao gồm và tất cả các bài kiểm tra hiện có đều vượt qua.
    - Đảm bảo mã của bạn tuân thủ hướng dẫn về kiểu dáng.
5. Thêm tên hoặc biệt danh của bạn vào Trang Giới thiệu:
    - Mở tệp `about_neobleeper.cs` và tìm thành phần `listView1`.
    - Chọn thành phần `listView1` trong trình thiết kế Visual Studio.
    - Nhấp vào mũi tên nhỏ ở góc trên bên phải của thành phần để mở menu thả xuống.
    - Chọn **Chỉnh sửa Mục** để mở trình chỉnh sửa bộ sưu tập mục ListView.
    - Thêm `ListViewItem` mới:
      - Nhập tên hoặc biệt danh của bạn vào thuộc tính **Văn bản**.
      - Đối với các đóng góp/nhiệm vụ của bạn:
        - Tìm thuộc tính **Mục con**.
        - Nhấp vào ba dấu chấm (`...`) ở bên phải trường `(Bộ sưu tập)`.
        - Thêm hoặc chỉnh sửa **SubItem** kèm theo mô tả ngắn gọn về nhiệm vụ của bạn.
    - Nếu bạn đã thêm tên, hãy chỉnh sửa SubItem hoặc cập nhật mục nhập hiện có trước khi cam kết thay đổi.
6. Kiểm tra mã của bạn:
    - Chạy thử nghiệm bằng Test Explorer của Visual Studio.
    - Sửa bất kỳ thử nghiệm nào không thành công và xác thực các thay đổi của bạn.
7. Cam kết thay đổi của bạn với các thông báo rõ ràng và súc tích.
    - Sử dụng các công cụ Git tích hợp của Visual Studio để dàn dựng và cam kết các thay đổi của bạn.
8. Đẩy nhánh của bạn và mở yêu cầu kéo trong kho lưu trữ.
9. Hãy sẵn sàng làm việc với người đánh giá và chỉnh sửa khi cần thiết.

### 🧾 Tài liệu
Cải thiện tài liệu của chúng tôi là một trong những cách dễ nhất để đóng góp! Bạn có thể thoải mái thêm hoặc cập nhật ví dụ, làm rõ các phần hoặc cải thiện khả năng đọc hiểu tổng thể.

### 🎼 Đóng góp tệp BMM và NBPML
NeoBleeper hỗ trợ các tệp BMM (Bleeper Music Maker) và NBPML (NeoBleeper Project Markup Language) cũ. Nếu bạn đang đóng góp hoặc làm việc với các loại tệp này, hãy đảm bảo những điều sau:
  - Xác thực rằng các tệp BMM được phân tích cú pháp chính xác và hiển thị như mong đợi trong NeoBleeper.
  - Kiểm tra khả năng tương thích với cả định dạng cũ và phiên bản hiện tại.
  - Đối với các tệp NBPML, hãy tuân thủ các thông số kỹ thuật Ngôn ngữ Đánh dấu Dự án NeoBleeper mới nhất.

Nếu bạn gặp bất kỳ sự cố nào liên quan đến các định dạng tệp này, vui lòng làm theo hướng dẫn trong phần "Báo cáo Lỗi". Chúng tôi cũng hoan nghênh các yêu cầu về tính năng hỗ trợ nâng cao cho các tệp BMM và NBPML!

## ⬇️ Quy trình Yêu cầu Kéo
Tất cả các yêu cầu gửi nên được thực hiện thông qua yêu cầu kéo. Quy trình như sau:
1. Điền vào mẫu yêu cầu kéo.
2. Đảm bảo yêu cầu kéo của bạn không trùng lặp với các yêu cầu hiện có.
3. Thêm chi tiết về các thay đổi của bạn vào phần mô tả, tham chiếu đến các vấn đề liên quan nếu có thể.
4. Giải quyết tất cả các bình luận hoặc yêu cầu thay đổi từ người đánh giá.
5. Yêu cầu kéo phải vượt qua tất cả các kiểm tra CI/CD, bao gồm các bài kiểm tra và kiểm tra chất lượng mã.

## 📖 Hướng dẫn Phong cách
### ✨ Phong cách Mã
Tuân thủ [Quy ước Lập trình .NET](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions). Các điểm chính bao gồm:
  - Ưu tiên các thuộc tính tự động hơn các trường công khai.
  - Sử dụng `var` cho các biến cục bộ khi kiểu dữ liệu rõ ràng.
  - Tránh sử dụng chuỗi ký tự và số. Sử dụng hằng số hoặc enum.

### 📒 Ghi chú cụ thể về C#
  - Đặt `{` trên cùng dòng với đoạn mã trước.
  - Sử dụng PascalCase cho tên lớp và tên phương thức, và camelCase cho các biến cục bộ.
  - Tuân thủ [Nguyên tắc Đặt tên của Microsoft](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines).

## 👨‍👩‍👧‍👦 Hỗ trợ Cộng đồng
Nếu có bất kỳ thắc mắc nào, vui lòng mở Thảo luận GitHub hoặc liên hệ qua các vấn đề. Chúng tôi khuyến khích mọi người chia sẻ kiến ​​thức và giúp đỡ những người cộng tác khác.

Cảm ơn bạn đã đóng góp cho NeoBleeper và cùng nhau xây dựng một điều tuyệt vời!
