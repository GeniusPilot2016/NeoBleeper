# Hướng dẫn Khắc phục Sự cố NeoBleeper

Hướng dẫn này cung cấp giải pháp cho các sự cố thường gặp khi sử dụng NeoBleeper, đặc biệt là các sự cố liên quan đến hoạt động của loa hệ thống, đầu ra âm thanh, khả năng tương thích phần cứng và tiếng bíp liên tục của hệ thống.

---

## 1. Âm thanh bị kẹt trong loa hệ thống sau khi gặp sự cố hoặc bị đóng mạnh

**Sự cố:**
Nếu NeoBleeper gặp sự cố hoặc bị đóng mạnh trong khi âm thanh đang phát qua loa hệ thống (PC), âm thanh có thể bị "kẹt", dẫn đến tiếng bíp hoặc tiếng ù liên tục.

**Nguyên nhân xảy ra:**
Loa hệ thống được điều khiển ở mức phần cứng/phần mềm thấp. Nếu ứng dụng không nhả hoặc đặt lại loa đúng cách khi thoát, tiếng bíp có thể vẫn tiếp diễn.

**Giải pháp:**
- **Sử dụng tiện ích NeoBleeper Nút Chặn Tiếng Bíp (cho phiên bản 64-bit):**
NeoBleeper đi kèm với một công cụ có tên "NeoBleeper Nút Chặn Tiếng Bíp" trong thư mục chương trình.

![image4](https://github.com/user-attachments/assets/3e46c8dd-62b9-4789-a7a0-ca245d365dce)

- Khởi chạy công cụ này và nhấn nút **Dừng tiếng bíp** để dừng tiếng bíp bị kẹt từ loa hệ thống.
- Chỉ sử dụng tiện ích này khi tiếng bíp vẫn tiếp tục sau khi gặp sự cố hoặc buộc thoát.

  #### Thông báo Dừng tiếng bíp và Ý nghĩa của chúng
  
    Khi sử dụng tiện ích Dừng tiếng bíp, bạn có thể thấy các thông báo sau:

  ![image1](https://github.com/user-attachments/assets/325b1c12-ef89-43fc-955d-f6e5a8e7ec58)
    
    **Loa hệ thống không phát ra tiếng bíp hoặc loa hệ thống phát ra tiếng bíp theo cách khác. Không có hành động nào được thực hiện.**  
    Thông báo này xuất hiện khi tiện ích kiểm tra loa hệ thống và xác định loa không phát ra tiếng bíp hoặc phát ra tiếng bíp theo cách mà công cụ không thể điều khiển. Trong trường hợp này, Nút Chặn Tiếng Bíp sẽ không thực hiện bất kỳ hành động nào nữa.
    - *Mẹo:* Nếu bạn vẫn nghe thấy tiếng bíp liên tục, hãy thử khởi động lại máy tính.

  ![image2](https://github.com/user-attachments/assets/0206de42-8933-4ad1-9c73-84215062a9d3)
    
    **Tiếng bíp đã dừng thành công!**  
    Thông báo này xác nhận rằng tiện ích Nút Chặn Tiếng Bíp đã phát hiện tiếng bíp bị kẹt và đã dừng thành công. Không cần thực hiện thêm thao tác nào nữa.

  ![image3](https://github.com/user-attachments/assets/9f9cf0c9-8845-45d3-8627-a680f0c83b93)
  
    **Không có đầu ra loa hệ thống hoặc đầu ra loa hệ thống không chuẩn. Bộ chặn tiếng bíp có thể gây mất ổn định hoặc các hiện tượng không mong muốn. Bạn có muốn tiếp tục không??**  
    Thông báo này xuất hiện khi tiện ích Nút Chặn Tiếng Bíp được khởi động và phát hiện hệ thống của bạn không có loa hệ thống chuẩn (PC) hoặc đầu ra loa hệ thống "không chuẩn". Trong trường hợp này, tiện ích cảnh báo bạn rằng việc cố gắng sử dụng Nút Chặn Tiếng Bíp có thể không hoạt động như mong đợi và có khả năng gây ra hành vi bất ngờ hoặc mất ổn định.

    Nếu bạn tiếp tục, công cụ sẽ cố gắng dừng tiếng bíp, nhưng việc này có thể không hiệu quả hoặc gây ra tác dụng phụ nếu phần cứng của bạn không được hỗ trợ hoặc không chuẩn.
    
    Nếu bạn chọn không tiếp tục, công cụ sẽ thoát mà không thực hiện bất kỳ thay đổi nào.
    - *Mẹo:* Nếu bạn nhận được thông báo này, điều đó có nghĩa là máy tính của bạn không có loa hệ thống tương thích hoặc đầu ra của loa không thể được kiểm soát một cách đáng tin cậy. Bất kỳ tiếng bíp hoặc tiếng ù nào bạn nghe thấy có thể đến từ một thiết bị âm thanh khác (chẳng hạn như loa chính hoặc tai nghe của bạn). Hãy sử dụng cài đặt thiết bị âm thanh tiêu chuẩn của bạn để giải quyết các sự cố âm thanh và đóng bất kỳ ứng dụng nào có thể đang tạo ra âm thanh không mong muốn. Nếu sự cố vẫn tiếp diễn, hãy thử khởi động lại máy tính hoặc kiểm tra cài đặt âm thanh của thiết bị.
      
- **Khởi động lại máy tính:**
  Nếu Nút Chặn Tiếng Bíp không giải quyết được sự cố, việc khởi động lại hệ thống sẽ thiết lập lại phần cứng loa.

- **Phòng ngừa:**
  Luôn đóng NeoBleeper một cách bình thường. Tránh buộc đóng nó bằng Trình quản lý tác vụ hoặc các công cụ tương tự khi âm thanh đang phát.

---

## 2. Phát hiện và Tương thích Loa Hệ thống
NeoBleeper bao gồm logic phát hiện để kiểm tra xem hệ thống của bạn có đầu ra loa PC tiêu chuẩn hay không, cũng như hỗ trợ đầu ra loa hệ thống "ẩn" (chẳng hạn như những đầu ra không sử dụng ID PNP0800). Nếu phần cứng của bạn không hỗ trợ loa hệ thống tiêu chuẩn hoặc ẩn, hoặc đầu ra không chuẩn và không sử dụng được, bạn có thể thấy thông báo cảnh báo hoặc phải dựa vào thiết bị âm thanh thông thường để phát ra tiếng bíp. Tuy nhiên, bắt đầu từ các phiên bản gần đây, NeoBleeper không còn bắt buộc bạn phải sử dụng thiết bị âm thanh độc quyền khi không có loa tiêu chuẩn nữa—giờ đây, nó cho phép sử dụng đầu ra loa hệ thống ẩn/không phải PNP0800 nếu có.

### Ví dụ cảnh báo (Hình 1):

![image1](https://github.com/user-attachments/assets/45efe32d-1253-4b8f-a8f1-aa8cbcd22853)

> **Giải thích:**
> Bo mạch chủ máy tính của bạn không có đầu ra loa hệ thống tiêu chuẩn, hoặc đầu ra không chuẩn. NeoBleeper sẽ cố gắng phát hiện và đề xuất sử dụng các đầu ra loa hệ thống "ẩn" không được xác định là PNP0800. Nếu có đầu ra như vậy, giờ đây bạn có thể sử dụng loa hệ thống ngay cả khi cảnh báo này xuất hiện. Nếu không, NeoBleeper sẽ chuyển sang thiết bị âm thanh thông thường của bạn (như loa hoặc tai nghe).

### Hộp thoại Cài đặt (Hình ảnh 2 và 3):

![image2](https://github.com/user-attachments/assets/5dc2839d-9ea2-4f00-90bf-2c51ef5a06a2)

![image3](https://github.com/user-attachments/assets/b901b053-26ff-4429-b656-515fd90cc6d1)

- **Tính khả dụng của nút "Kiểm tra Loa Hệ thống":**
  Tùy chọn này được bật nếu NeoBleeper phát hiện bất kỳ đầu ra loa hệ thống nào có thể sử dụng được, bao gồm cả đầu ra ẩn hoặc không phải PNP0800.
- **Cài đặt "Sử dụng thiết bị âm thanh để tạo tiếng bíp":**
  Giờ đây, bạn được phép tắt tính năng này nếu phát hiện đầu ra loa hệ thống ẩn hoặc không chuẩn.

#### "Đầu ra loa hệ thống không chuẩn" nghĩa là gì?
Một số máy tính, máy tính xách tay hoặc máy ảo hiện đại không có loa PC thực sự, hoặc đường truyền tín hiệu không chuẩn. NeoBleeper hiện đang cố gắng phát hiện và sử dụng các đầu ra loa hệ thống ẩn này (không được xác định là thiết bị PNP0800), nhưng chỉ có thể bật tùy chọn loa hệ thống nếu nó thực sự có thể truy cập được ở cấp độ phần cứng. Nếu không tìm thấy đầu ra nào khả dụng, bạn sẽ cần sử dụng thiết bị âm thanh thông thường của mình.

## 2.1 Kiểm tra Đầu ra Loa Hệ thống (Phát hiện Tần số Siêu âm)
NeoBleeper hiện bao gồm một bài kiểm tra phần cứng mới, nâng cao để phát hiện đầu ra loa hệ thống (hay còn gọi là loa PC), ngay cả khi thiết bị không được Windows báo cáo (trong một số ID nhất định như PNP0C02 thay vì PNP0800). Bài kiểm tra này sử dụng tần số siêu âm (thường là 30–38 kHz, không thể nghe thấy) và phân tích phản hồi điện trên cổng loa hệ thống.

- **Cách thức hoạt động:**
  Trong quá trình khởi động, NeoBleeper thực hiện bước thứ hai sau khi kiểm tra ID thiết bị thông thường. Nó gửi tín hiệu siêu âm đến cổng loa hệ thống và theo dõi phản hồi phần cứng để phát hiện sự hiện diện của đầu ra loa đang hoạt động—ngay cả khi bị ẩn hoặc không chuẩn.

- **Những điều bạn có thể nhận thấy:**
  Trên một số hệ thống, đặc biệt là những hệ thống có còi áp điện, bạn có thể nghe thấy tiếng lách cách nhỏ trong giai đoạn này. Điều này là bình thường và cho biết bài kiểm tra phần cứng đang chạy.
  ![image4](https://github.com/user-attachments/assets/81a8d6d8-565e-4fd3-aefc-6122c06d17a9)
  *Đang kiểm tra sự hiện diện của đầu ra loa hệ thống (loa máy tính) ở bước 2/2... (bạn có thể nghe thấy tiếng tách tách)*

- **Tại sao phải kiểm tra?**
  Nhiều hệ thống hiện đại không có thiết bị loa hệ thống PNP0800, nhưng vẫn có đầu ra loa có thể sử dụng được (ẩn). NeoBleeper sử dụng phương pháp tiên tiến này để bật tính năng phát tiếng bíp trên nhiều phần cứng hơn.

---

## 3. Hỗ trợ và Hạn chế ARM64

**Thiết bị chạy ARM64:**
Trên các hệ thống Windows ARM64, tính năng kiểm tra "Loa Hệ thống" và hộp kiểm "Sử dụng thiết bị âm thanh để tạo tiếng bíp" **không khả dụng** trong NeoBleeper. Thay vào đó, tất cả tiếng bíp và đầu ra âm thanh luôn được tạo ra thông qua thiết bị âm thanh tiêu chuẩn của bạn (loa hoặc tai nghe).

- Nút "Kiểm tra Loa Hệ thống" và các tính năng phát hiện liên quan sẽ **không** hiển thị trong phần cài đặt trên các thiết bị ARM64.
- Tùy chọn "Sử dụng thiết bị âm thanh để tạo tiếng bíp" không có sẵn vì hành vi này được áp dụng tự động.
- Hạn chế này tồn tại vì không thể truy cập trực tiếp vào phần cứng loa máy tính/hệ thống trên nền tảng ARM64 Windows.
- Bạn sẽ luôn nghe thấy tiếng bíp thông qua thiết bị đầu ra âm thanh thông thường trên ARM64.

**Nếu bạn đang sử dụng máy ARM64 và không thấy các tùy chọn loa hệ thống trong NeoBleeper, điều này là bình thường và không phải là lỗi.**

---

## 4. Cách kiểm tra sự hiện diện của loa hệ thống

- **Máy tính để bàn:** Hầu hết các máy tính để bàn cũ đều có đầu cắm loa PC trên bo mạch chủ. Các hệ thống mới hơn có thể bỏ qua tính năng này hoặc có thể hiển thị đầu ra ở dạng ẩn/không phải PNP0800 mà NeoBleeper hiện có thể sử dụng.
- **Máy tính xách tay:** Hầu hết máy tính xách tay không có loa hệ thống riêng; tất cả âm thanh đều được định tuyến qua hệ thống âm thanh chính.
- **Máy ảo:** Tính năng mô phỏng loa hệ thống thường không có hoặc không đáng tin cậy; các đầu ra không phải PNP0800 có thể không khả dụng.
- **Cách nhận biết:** Nếu bạn thấy các cảnh báo ở trên nhưng có thể bật và kiểm tra loa hệ thống trong NeoBleeper, máy tính của bạn có thể có đầu ra ẩn hoặc không chuẩn.

---

## 5. Tôi không nghe thấy âm thanh nào!
- **Kiểm tra cài đặt NeoBleeper của bạn:**
  Nếu loa hệ thống của bạn không khả dụng, hãy đảm bảo thiết bị âm thanh (loa/tai nghe) được chọn chính xác và đang hoạt động.
- **Kiểm tra bộ trộn âm lượng của Windows:**
  Đảm bảo NeoBleeper không bị tắt tiếng trong bộ trộn âm lượng của hệ thống.
- **Hãy thử nút "Kiểm tra loa hệ thống":**
  Sử dụng nút này để kiểm tra loa máy tính của bạn.
- **Đọc thông báo cảnh báo:**
  NeoBleeper sẽ cung cấp hướng dẫn cụ thể nếu không thể truy cập loa hệ thống của bạn.

---

## 6. Cảnh báo, Lỗi AI và Khắc phục sự cố API Google Gemini™

Tính năng "Sáng tác nhạc bằng AI" của NeoBleeper sử dụng API Google Gemini™. Bạn có thể gặp phải các hộp thoại lỗi hoặc cảnh báo cụ thể liên quan đến tính khả dụng của API, giới hạn sử dụng hoặc hạn chế quốc gia.

### 6.1 Lỗi Hạn mức hoặc Giới hạn Tốc độ (429 RESOURCE_EXHAUSTED)

![image1](https://github.com/user-attachments/assets/92bb28a3-f30e-4e1f-9759-2fad99d935b4)

**Thông báo Ví dụ:**
```
Đã xảy ra lỗi: RESOURCE_EXHAUSTED (Code: 429): You exceeded your current quota, please check your plan and billing details...
```

**Nguyên nhân tiềm ẩn:**
- **Hạn mức API cho tài khoản của bạn đã hết.** Nếu bạn đang sử dụng khóa API miễn phí, một số mô hình (chẳng hạn như `gemini-2.0-pro-exp`) có thể không khả dụng hoặc có thể có giới hạn sử dụng rất thấp/cứng đối với tài khoản miễn phí.
- **Giới hạn cấp miễn phí:** Một số mô hình tạo mới hơn (như Gemini Pro Exp) *không* khả dụng đối với người dùng cấp miễn phí. Việc cố gắng sử dụng chúng sẽ dẫn đến lỗi hạn ngạch hoặc lỗi khả dụng.
- **Vượt quá giới hạn tốc độ:** Nếu bạn gửi quá nhiều yêu cầu quá nhanh, bạn có thể đạt đến giới hạn tốc độ của API ngay cả khi sử dụng gói trả phí.

**Cách khắc phục:**
- **Kiểm tra hạn ngạch API và hóa đơn của bạn:** Đăng nhập vào tài khoản Google Cloud/Gemini của bạn để xác minh mức sử dụng và nâng cấp gói nếu cần.
- **Chỉ sử dụng các mô hình được hỗ trợ:** Người dùng gói miễn phí có thể bị giới hạn ở một số mô hình nhất định. Kiểm tra tài liệu về các mô hình khả dụng hoặc chuyển sang mô hình được hỗ trợ.
- **Chờ và thử lại sau:** Đôi khi, việc chờ một vài phút sẽ cho phép hạn ngạch tạm thời làm mới, như được chỉ ra bởi bộ đếm ngược của thông báo.

- **Xem lại [Tài liệu API Gemini](https://ai.google.dev/gemini-api/docs/rate-limits) để biết các chính sách sử dụng và giới hạn tốc độ mới nhất.**

---

### 6.2 Khắc phục sự cố cho các mô hình Gemini rất mới hoặc chưa được ghi chép (ví dụ: Bản xem trước Gemini 3 Pro)

Một số mô hình Gemini—đặc biệt là các bản phát hành hoàn toàn mới như **Bản xem trước Gemini 3 Pro**—có thể không xuất hiện trong tài liệu định giá hoặc hạn ngạch API Gemini chính thức khi ra mắt. Bạn có thể gặp lỗi hạn ngạch, quyền truy cập hoặc lỗi "RESOURCE_EXHAUSTED" ngay cả khi hạn ngạch tài khoản tổng thể của bạn dường như chưa được sử dụng.

**Những cân nhắc quan trọng đối với các mô hình rất mới:**
- Google thường giới hạn quyền truy cập vào các mô hình xem trước (như Bản xem trước Gemini 3 Pro) cho một số tài khoản hoặc khu vực cụ thể và có thể áp dụng các giới hạn yêu cầu và sử dụng nghiêm ngặt hơn nhiều.
- Tài khoản miễn phí có thể không có hạn ngạch cho các mô hình này hoặc các yêu cầu có thể bị chặn hoàn toàn.
- Mô hình có thể không hiển thị trong các tab hạn ngạch/giá hoặc tài liệu của Google trong vài tuần sau khi phát hành.
- Giá cả, quyền truy cập và tính khả dụng của các mô hình Gemini mới có thể thay đổi thường xuyên.

**Cần làm gì nếu gặp lỗi:**
- Kiểm tra kỹ [mức sử dụng API và hạn ngạch](https://ai.dev/usage?tab=rate-limit) của bạn và xem mô hình mới có xuất hiện trong bảng điều khiển của bạn hay không.
- Xem lại [tài liệu API Gemini](https://ai.google.dev/gemini-api/docs/rate-limits), nhưng lưu ý rằng tài liệu có thể chậm hơn so với các mô hình mới phát hành.
- Nếu bạn thấy lỗi như "RESOURCE_EXHAUSTED" đối với một mô hình không được ghi trong bảng giá chính thức, điều đó có thể có nghĩa là mô hình đó chưa được cung cấp rộng rãi hoặc có quyền truy cập xem trước rất hạn chế.
- Chờ Google cập nhật tài liệu của họ và triển khai rộng rãi hơn nếu bạn cần sử dụng các mô hình thử nghiệm này.

> **Lưu ý:**
> NeoBleeper và các ứng dụng tương tự không thể vượt qua những hạn chế này. Nếu tài khoản hoặc khu vực của bạn không đủ điều kiện, bạn phải đợi cho đến khi Google chính thức cho phép truy cập hoặc tăng hạn ngạch cho mô hình Gemini bạn đã chọn.

---

### 6.3 Giới hạn theo Khu vực hoặc Quốc gia

#### "API không khả dụng tại quốc gia của bạn"

![image4](https://github.com/user-attachments/assets/278c1e00-c1eb-4a10-9a82-f55eb3d55703)

Một số khu vực không được hỗ trợ API Google Gemini™ do các hạn chế về khu vực hoặc pháp lý.

**Nguyên nhân tiềm ẩn:**
- Quốc gia của bạn là quốc gia mà API Gemini bị hạn chế.
- Khóa API bạn đang sử dụng được đăng ký cho một khu vực không có quyền truy cập.

**Cách khắc phục:**
- **Kiểm tra các quốc gia được hỗ trợ API Google Gemini™** trong tài liệu chính thức.
- Nếu bạn ở quốc gia bị hạn chế, các tính năng AI sẽ không thể sử dụng được.

#### Cảnh báo theo khu vực (Bảng Cài đặt)

![image3](https://github.com/user-attachments/assets/fa282623-685a-4e59-b124-eb47a2173451)

Tại Khu vực Kinh tế Châu Âu, Thụy Sĩ hoặc Vương quốc Anh, API Gemini™ có thể yêu cầu tài khoản Google trả phí (không miễn phí).

- Nếu bạn thấy cảnh báo này, hãy đảm bảo bạn đã nâng cấp gói API Gemini trước khi thử sử dụng các tính năng AI.

---

### 6.4 Lời khuyên chung về API AI

- Chỉ nhập khóa API của riêng bạn; không chia sẻ khóa này vì lý do bảo mật.
- NeoBleeper không truyền khóa API của bạn, ngoại trừ trực tiếp đến dịch vụ Gemini khi cần sử dụng tính năng.
- Nếu bạn gặp lỗi lặp lại, hãy thử xóa và thêm lại khóa API, đồng thời kiểm tra lại xem khóa của bạn có đang hoạt động không.

---

## 7. Tư vấn về Loa Hệ thống và Âm thanh cho một số Chipset Cụ thể (bao gồm Intel B660)

### Nếu bạn không nghe thấy âm thanh, âm thanh bị hỏng hoặc loa hệ thống không ổn định:

Một số chipset hiện đại — bao gồm các chipset thuộc dòng Intel B660 và mới hơn — có thể gặp sự cố khi khởi tạo hoặc khởi tạo lại loa hệ thống (tiếng bíp PC), dẫn đến tình trạng máy tính im lặng hoặc không có tiếng.

**Lời khuyên dành cho người dùng bị ảnh hưởng:**

- **Thử đặt máy tính ở chế độ ngủ và đánh thức lại.**

Điều này có thể giúp khởi tạo lại hoặc đặt lại cổng phần cứng cấp thấp chịu trách nhiệm cho loa hệ thống và khôi phục chức năng tiếng bíp.
- **Sử dụng tính năng "Sử dụng thiết bị âm thanh để tạo tiếng bíp"** như một giải pháp dự phòng nếu đầu ra loa hệ thống không ổn định.
- **Kiểm tra bản cập nhật BIOS hoặc firmware:** Một số nhà cung cấp bo mạch chủ có thể phát hành các bản cập nhật cải thiện khả năng tương thích của cổng loa.
- **Dành riêng cho máy tính để bàn:** Nếu bạn đã thêm, tháo hoặc kết nối lại phần cứng loa hệ thống, hãy thực hiện chu kỳ bật/tắt nguồn hoàn toàn.

_Giải pháp thay thế này được nêu bật trong phần cài đặt:_

![image2](https://github.com/user-attachments/assets/ea02721e-5f69-467f-a0e7-30a8b22e39af)

> *Nếu bạn không nghe thấy âm thanh hoặc âm thanh bị hỏng, hãy thử đặt máy tính ở chế độ ngủ và đánh thức lại. Điều này có thể giúp khởi tạo lại loa hệ thống trên các chipset bị ảnh hưởng.*

---

*Đối với bất kỳ sự cố nào liên quan đến âm thanh hoặc AI không được đề cập ở đây, vui lòng bao gồm ảnh chụp màn hình lỗi, thông tin chi tiết về phần cứng máy tính của bạn (đặc biệt là hãng và model bo mạch chủ/chipset), cũng như quốc gia/khu vực của bạn khi yêu cầu hỗ trợ hoặc mở sự cố GitHub.*

---

## 6. Câu hỏi thường gặp

### H: Tôi có thể sử dụng loa hệ thống nếu phần cứng của tôi không có thiết bị PNP0800 không?
**A:** Có! NeoBleeper hiện đang cố gắng phát hiện và sử dụng các đầu ra loa hệ thống ẩn hoặc không phải PNP0800 nếu có thể. Nếu thành công, bạn có thể sử dụng loa hệ thống ngay cả khi Windows không báo cáo thiết bị tiêu chuẩn.

### H: Tại sao cài đặt "Sử dụng thiết bị âm thanh để tạo tiếng bíp" đôi khi trở thành cài đặt cố định (trong các phiên bản cũ hơn)?
**A:** Khi không phát hiện thấy đầu ra loa hệ thống tiêu chuẩn (trong các phiên bản cũ hơn), NeoBleeper sẽ áp dụng cài đặt này để đảm bảo vẫn có thể phát ra âm thanh.

### H: Có cách nào khắc phục tình trạng loa hệ thống bị mất không?
**A:** Bạn phải sử dụng thiết bị âm thanh thông thường (loa/tai nghe) nếu không tìm thấy đầu ra loa hệ thống tiêu chuẩn (trong các phiên bản cũ hơn).

### H: Điều gì sẽ xảy ra nếu công cụ Chặn Tiếng Bíp không dừng được tiếng bíp bị kẹt?
**A:** Khởi động lại máy tính để thiết lập lại phần cứng loa nếu tiện ích Nút Chặn Tiếng Bíp bị lỗi.

### H: Tại sao tôi nghe thấy tiếng lách cách khi khởi động?
**A:** Trong quá trình kiểm tra đầu ra loa hệ thống nâng cao (bước 2), NeoBleeper gửi tín hiệu siêu âm đến phần cứng để phát hiện các đầu ra loa ẩn hoặc không chuẩn. Trên một số hệ thống (đặc biệt là những hệ thống có còi áp điện), điều này có thể gây ra tiếng lách cách yếu ớt. Điều này là bình thường và không phải là dấu hiệu của sự cố; nó chỉ đơn giản có nghĩa là bài kiểm tra phần cứng đang chạy.

### H: Bài kiểm tra phần cứng siêu âm (bước 2) có thể phát hiện loa hệ thống bị hỏng (hở mạch) hoặc bị ngắt kết nối không?
**A:** Hiện tại, điều này chưa được kiểm tra và chưa rõ. Mặc dù bài kiểm tra này kiểm tra phản hồi điện và hoạt động của cổng, nhưng nó có thể không phân biệt chính xác giữa loa hiện diện nhưng bị hỏng (hở mạch) hoặc bị ngắt kết nối và loa bị mất. Nếu loa bị hỏng hoàn toàn hoặc bị ngắt kết nối (hở mạch), bài kiểm tra có thể trả về kết quả sai, cho biết không phát hiện thấy đầu ra hoạt động. Tuy nhiên, kết quả này không được đảm bảo và có thể phụ thuộc vào phần cứng và chế độ lỗi cụ thể. Nếu bạn nghi ngờ loa hệ thống của mình không hoạt động, hãy kiểm tra thực tế hoặc sử dụng đồng hồ vạn năng.

### H: Tại sao tôi không thấy bất kỳ tùy chọn loa hệ thống hoặc âm thanh bíp nào trên thiết bị ARM64 của mình?
**A:** Trên hệ thống Windows ARM64, NeoBleeper vô hiệu hóa các cài đặt liên quan đến loa hệ thống vì nền tảng ARM64 không hỗ trợ truy cập trực tiếp vào phần cứng loa hệ thống. Tất cả tiếng bíp đều được phát qua thiết bị đầu ra âm thanh thông thường của bạn (loa hoặc tai nghe), và các tùy chọn "Kiểm tra Loa Hệ thống" và "Sử dụng thiết bị âm thanh để tạo tiếng bíp" sẽ tự động bị ẩn. Hiện tượng này là do thiết kế chứ không phải lỗi.

### H: Khi tôi nhận được cảnh báo "có đầu ra loa hệ thống không chuẩn" thì điều đó có nghĩa là gì?
**A:** NeoBleeper đã phát hiện phần cứng loa không tuân thủ các tiêu chuẩn loa PC truyền thống (ví dụ: không phải thiết bị PNP0800). Đây có thể là đầu ra loa "ẩn" được tìm thấy trên máy tính để bàn hoặc máy ảo hiện đại. Trong những trường hợp này, không phải tất cả các tính năng phát tiếng bíp đều hoạt động đáng tin cậy, nhưng NeoBleeper sẽ cố gắng sử dụng bất kỳ đầu ra tương thích nào mà nó có thể phát hiện.

### H: Tại sao nút "Kiểm tra Loa Hệ thống" lại xuất hiện ngay cả khi Windows không liệt kê thiết bị loa PC?
**A:** NeoBleeper bao gồm logic phát hiện cho các đầu ra loa hệ thống ẩn hoặc không chuẩn. Nếu nút này xuất hiện, điều đó có nghĩa là NeoBleeper đã tìm thấy một cổng phần cứng tiềm năng cho đầu ra loa, ngay cả khi Windows không báo cáo nó là một thiết bị.

### H: Tôi đang sử dụng API Google Gemini™ cho các tính năng AI và tôi thấy thông báo "hết hạn ngạch" hoặc "API không khả dụng ở quốc gia của bạn". Tôi nên làm gì?
**A:** Tham khảo mục 6 của hướng dẫn này. Đảm bảo khóa API và hóa đơn/hạn mức của bạn đang hoạt động tốt, đồng thời việc sử dụng của bạn tuân thủ các hạn chế khu vực của Google. Rất tiếc, nếu bạn ở trong khu vực bị hạn chế, các tính năng AI có thể không khả dụng.

### H: Tôi có hệ thống Intel B660 (hoặc mới hơn) và loa máy tính của tôi đôi khi không hoạt động hoặc bị kẹt. Điều này có bình thường không?
**A:** Một số chipset mới hơn có vấn đề tương thích đã biết khi khởi động lại loa hệ thống. Hãy thử để máy tính ở chế độ ngủ và đánh thức lại, hoặc sử dụng thiết bị âm thanh thông thường của bạn. Kiểm tra các bản cập nhật BIOS/firmware có thể cải thiện khả năng hỗ trợ loa.

### H: Cách tốt nhất để báo cáo sự cố âm thanh hoặc AI để được hỗ trợ là gì?
**A:** Luôn cung cấp càng nhiều thông tin càng tốt: hãng/model máy tính, khu vực, ảnh chụp màn hình hộp thoại lỗi và tệp `DebugLog.txt` từ thư mục NeoBleeper. Đối với các sự cố AI, hãy bao gồm toàn bộ văn bản hộp thoại lỗi và mô tả trạng thái tài khoản Gemini API của bạn.

### H: Sau khi gặp sự cố hoặc buộc đóng, tính năng Beep Stopper của NeoBleeper không dừng tiếng bíp liên tục. Có cách nào khác để khắc phục sự cố này không?
**A:** Nếu tính năng Beep Stopper không hiệu quả, việc khởi động lại máy tính sẽ đặt lại phần cứng loa hệ thống và dừng mọi tiếng bíp liên tục.

### H: Có an toàn khi sử dụng tiện ích Beep Stopper nếu tôi thấy thông báo cảnh báo về đầu ra loa hệ thống không chuẩn hoặc bị thiếu không?
**A:** Có, nhưng lưu ý rằng tiện ích này có thể không kiểm soát được phần cứng và trong một số trường hợp hiếm gặp có thể gây mất ổn định hoặc không có tác dụng. Nếu bạn không chắc chắn, hãy chọn không tiếp tục và khởi động lại máy tính.

### H: Trên máy ảo, tôi không thể khiến loa hệ thống hoạt động. Đây có phải là lỗi không?
**A:** Không nhất thiết. Nhiều máy ảo không mô phỏng đúng loa PC hoặc chúng hiển thị đầu ra theo cách không thể điều khiển bằng lập trình. Hãy sử dụng thiết bị đầu ra âm thanh chuẩn của bạn để có kết quả tốt nhất.

**Các bản cập nhật tiềm năng trong tương lai:**
Nếu các thử nghiệm hoặc phát triển trong tương lai cho phép NeoBleeper phát hiện loa hệ thống bị hỏng hoặc ngắt kết nối một cách đáng tin cậy thông qua kiểm tra phần cứng siêu âm, Câu hỏi thường gặp này và logic phát hiện sẽ được cập nhật để phản ánh những cải tiến đó. Vui lòng theo dõi nhật ký thay đổi hoặc bản phát hành mới để biết thêm chi tiết.

---

## 7. Nhận trợ giúp

- **Cung cấp thông tin chi tiết về máy tính và môi trường:** Khi báo cáo sự cố phát hiện phần cứng hoặc âm thanh, vui lòng bao gồm thông tin chi tiết về máy tính của bạn (máy tính để bàn/máy tính xách tay, nhà sản xuất/mẫu máy, hệ điều hành) và bất kỳ phần cứng nào có liên quan.
- **Đính kèm ảnh chụp màn hình hoặc hộp thoại báo lỗi:** Ảnh chụp màn hình hộp thoại báo lỗi hoặc cảnh báo rất hữu ích. Vui lòng chỉ rõ thời điểm sự cố xảy ra.
- **Bao gồm tệp nhật ký:** Bắt đầu từ các phiên bản mới hơn, NeoBleeper sẽ tạo một tệp nhật ký chi tiết có tên `DebugLog.txt` trong thư mục chương trình. Vui lòng đính kèm tệp này khi cần trợ giúp, vì tệp này chứa thông tin chẩn đoán hữu ích.
- **Mô tả các bước để tái tạo sự cố:** Nêu rõ những gì bạn đang làm khi sự cố xảy ra.
- **Mở một sự cố trên GitHub:** Để được hỗ trợ thêm, hãy mở một sự cố trên GitHub và bao gồm tất cả các chi tiết trên để được hỗ trợ tốt nhất.

_Hướng dẫn này được cập nhật khi phát hiện ra các sự cố và giải pháp mới. Để được hỗ trợ thêm, vui lòng mở một sự cố trên GitHub kèm theo thông tin chi tiết về thiết lập của bạn và sự cố gặp phải._
