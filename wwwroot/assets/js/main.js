document.addEventListener("DOMContentLoaded", function () {
  const deleteButtons = document.querySelectorAll(".btn-danger");
  deleteButtons.forEach((btn) => {
    btn.addEventListener("click", function (e) {
      const ok = confirm("Bạn có chắc muốn xóa mục này không?");
      if (!ok) e.preventDefault();
    });
  });

  const saveButtons = document.querySelectorAll("[data-demo-save]");
  saveButtons.forEach((btn) => {
    btn.addEventListener("click", function () {
      alert("Đã lưu cấu hình thành công (demo).");
    });
  });

  const fakeSubmitButtons = document.querySelectorAll("[data-demo-submit]");
  fakeSubmitButtons.forEach((btn) => {
    btn.addEventListener("click", function () {
      alert("Thao tác thành công (demo).");
    });
  });
});

window.addEventListener("load", function () {
  const loader = document.getElementById("page-loader");
  const content = document.querySelector(".page-content");

  setTimeout(() => {
    if (loader) loader.classList.add("hide");
    if (content) content.classList.add("show");
  }, 1500);
});