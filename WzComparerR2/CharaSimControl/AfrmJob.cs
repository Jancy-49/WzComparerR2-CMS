using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using CharaSimResource;
using WzComparerR2.WzLib;
using WzComparerR2.CharaSim;
using WzComparerR2.Common;
using WzComparerR2.Controls;
using WzComparerR2.PluginBase;
using System.Security.Cryptography;

namespace WzComparerR2.CharaSimControl
{
    public class AfrmJob : AlphaForm
    {
        public AfrmJob()
        {
            this.AllowDrop = true;
            initCtrl();
            this.job_list = new List<int>()
            {
            110, 120, 130, 210, 220, 230, 310, 320, 330, 410, 420, 430, 510, 520, 530, 1100, 1200, 1300, 1400, 1500, 2100, 2200, 2300, 2400, 2500, 2700,
            3100, 3101, 3200, 3300, 3500, 3600, 3700, 4100, 4200, 5100, 6100, 6300, 6400, 6500, 10100, 14200, 15100, 15200, 15400, 15500, 16200, 16400, 17200, 17500
            };
        }
        private Point baseOffset;
        private Point newLocation;
        private bool waitForRefresh;
        private Character character;

        private ACtrlHScroll hScroll;
        private ACtrlButton btnYes;
        private ACtrlButton btnNo;
        private List<ACtrlButton> btnSelectedJobs = new List<ACtrlButton>();

        public int jobIndex = 0;
        public int selectIndex = 0;
        private int pageIndex = 0;
        public List<int> job_list = new List<int>();


        public Character Character
        {
            get { return character; }
            set { character = value; }
        }

        private void initCtrl()
        {
            this.hScroll = new ACtrlHScroll();  //小屏鼠标滑轮区域

            this.hScroll.PicBase.Normal = new BitmapOrigin(Resource.ClassSelect_list_scrollmovePlus1_enabled_base);
            this.hScroll.PicBase.Disabled = new BitmapOrigin(Resource.ClassSelect_list_scrollmovePlus1_enabled_base);

            this.hScroll.BtnPrev.Normal = new BitmapOrigin(Resource.Login_img_ClassSelect_list_scroll_movePlus1_enabled_prev0);
            this.hScroll.BtnPrev.Pressed = new BitmapOrigin(Resource.Login_img_ClassSelect_list_scroll_movePlus1_enabled_prev0);
            this.hScroll.BtnPrev.MouseOver = new BitmapOrigin(Resource.Login_img_ClassSelect_list_scroll_movePlus1_enabled_prev0);
            this.hScroll.BtnPrev.Size = this.hScroll.BtnPrev.Normal.Bitmap.Size;
            this.hScroll.BtnPrev.Location = new Point(0, 0);

            this.hScroll.BtnNext.Normal = new BitmapOrigin(Resource.Login_img_ClassSelect_list_scroll_movePlus1_enabled_prev0);
            this.hScroll.BtnNext.Pressed = new BitmapOrigin(Resource.Login_img_ClassSelect_list_scroll_movePlus1_enabled_prev0);
            this.hScroll.BtnNext.MouseOver = new BitmapOrigin(Resource.Login_img_ClassSelect_list_scroll_movePlus1_enabled_prev0);
            this.hScroll.BtnNext.Size = this.hScroll.BtnNext.Normal.Bitmap.Size;
            this.hScroll.BtnNext.Location = new Point(957, 0);

            this.hScroll.BtnThumb.Normal = new BitmapOrigin(Resource.ClassSelect_list_scrollmovePlus1_enabled_thumb0);
            this.hScroll.BtnThumb.Pressed = new BitmapOrigin(Resource.ClassSelect_list_scrollmovePlus1_enabled_thumb1);
            this.hScroll.BtnThumb.MouseOver = new BitmapOrigin(Resource.ClassSelect_list_scrollmovePlus1_enabled_thumb2);
            this.hScroll.BtnThumb.Size = this.hScroll.BtnThumb.Normal.Bitmap.Size;

            this.hScroll.Location = new Point(378, 680);
            this.hScroll.Size = new Size(957, 5);
            this.hScroll.ScrollableLocation = new Point(378, 680);
            this.hScroll.ScrollableSize = new Size(957, 5);
            this.hScroll.Visible = true;
            this.hScroll.ValueChanged += new EventHandler(hScroll_ValueChanged);
            this.hScroll.ChildButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnYes = new ACtrlButton();
            this.btnYes.Normal = new BitmapOrigin(Resource.Login_img_Notice_New_notice_image_0_buttonYes_normal_0);
            this.btnYes.MouseOver = new BitmapOrigin(Resource.Login_img_Notice_New_notice_image_0_buttonYes_mouseOver_0);
            this.btnYes.Pressed = new BitmapOrigin(Resource.Login_img_Notice_New_notice_image_0_buttonYes_pressed_0);
            this.btnYes.Disabled = new BitmapOrigin(Resource.Login_img_Notice_New_notice_image_0_buttonYes_disabled_0);
            this.btnYes.Location = new Point(498, 700);
            this.btnYes.Size = new Size(160, 43);
            this.btnYes.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnYes.MouseClick += new MouseEventHandler(btnYes_MouseClick);

            this.btnNo = new ACtrlButton();
            this.btnNo.Normal = new BitmapOrigin(Resource.Login_img_Notice_New_notice_image_1_buttonNo_normal_0);
            this.btnNo.MouseOver = new BitmapOrigin(Resource.Login_img_Notice_New_notice_image_1_buttonNo_mouseOver_0);
            this.btnNo.Pressed = new BitmapOrigin(Resource.Login_img_Notice_New_notice_image_1_buttonNo_pressed_0);
            this.btnNo.Disabled = new BitmapOrigin(Resource.Login_img_Notice_New_notice_image_1_buttonNo_disabled_0);
            this.btnNo.Location = new Point(708, 700);
            this.btnNo.Size = new Size(160, 43);
            this.btnNo.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnNo.MouseClick += new MouseEventHandler(btnNo_MouseClick);

            btns();
            for (int i = 0; i < btnSelectedJobs.Count; i++)
            {
                ACtrlButton btnSelectedJob = btnSelectedJobs[i];
                btnSelectedJob.MouseOver = new BitmapOrigin(Resource.ClassSelect_list_class_layerclassCover);
                btnSelectedJob.Size = new Size(152, 74);
                btnSelectedJob.Visible = true;
                btnSelectedJob.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
                btnSelectedJob.MouseClick += new MouseEventHandler(btnSelectedJob_MouseClick);
            }
        }

        private void btns()
        {
            for (int i = 0; i < 42; i++)
            {
                var btnSelectedJob = new ACtrlButton();
                btnSelectedJob.Location = new Point(375 + 161 * (i / 7), 113 + 81 * (i % 7));
                btnSelectedJobs.Add(btnSelectedJob);
            }
        }

        public override void Refresh()
        {
            this.preRender();
            this.SetBitmap(this.Bitmap);
            this.CaptionRectangle = new Rectangle(this.baseOffset, new Size(Resource.ClassSelect_back_0_1_0.Width, Resource.ClassSelect_back_0_1_0.Height));
            this.Location = newLocation;
            base.Refresh();
        }

        private void preRender()
        {
            if (Bitmap != null)
                Bitmap.Dispose();
            Size size = Resource.ClassSelect_back_0_1_0.Size;

            //处理偏移
            this.newLocation = new Point(this.Location.X + this.baseOffset.X,
                this.Location.Y + this.baseOffset.Y);

            //绘制图像
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bitmap);
            renderBase(g);

            g.Dispose();
            this.Bitmap = bitmap;
        }

        private void renderBase(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.ClassSelect_back_0_1_0, 0, 0);
            var charObj = Resource.ResourceManager.GetObject("ClassSelect_back_1_" + job_list[selectIndex].ToString() + "_0");
            Bitmap charBitmap = charObj as Bitmap;
            if (charBitmap != null) // 确保转换成功
            {
                g.DrawImage(charBitmap, (1366 - charBitmap.Width) / 2, 0);
            }
            g.DrawImage(Resource.ClassSelect_layeraboveSpine, 0, 0);
            g.DrawImage(Resource.ClassSelect_back_2_0, 50, 13);
            g.DrawImage(Resource.ClassSelect_list_backgrnd1_0_0, 378, 81);
            string subName = PluginManager.FindWz($@"UI/Login.img/ClassSelect/desc/info/" + job_list[selectIndex].ToString() + "/subName").GetValueEx<string>(null);
            string desc = PluginManager.FindWz($@"UI/Login.img/ClassSelect/desc/info/" + job_list[selectIndex].ToString() + "/desc").GetValueEx<string>(null).Replace("\\n", "\r\n");
            string race = PluginManager.FindWz($@"UI/Login.img/ClassSelect/desc/info/" + job_list[selectIndex].ToString() + "/race").GetValueEx<string>(null).Replace("\\n", "\r\n");
            string move = PluginManager.FindWz($@"UI/Login.img/ClassSelect/desc/info/" + job_list[selectIndex].ToString() + "/move").GetValueEx<string>(null).Replace("\\n", "\r\n");
            string stat = PluginManager.FindWz($@"UI/Login.img/ClassSelect/desc/info/" + job_list[selectIndex].ToString() + "/stat").GetValueEx<string>(null).Replace("\\n", "\r\n");
            g.DrawString(subName, GearGraphics.ClassSelectFontBold, GearGraphics.WhiteBrush, 48f, 223f);
            int picH = 263;
            GearGraphics.DrawPlainText(g, desc, GearGraphics.ClassSelectDescFont, Color.FromArgb(255, 255, 255), 50, 345, ref picH, 16);
            g.DrawString(race, GearGraphics.ClassSelectDescFont, GearGraphics.WhiteBrush, 143f, 383f);
            g.DrawString(move, GearGraphics.ClassSelectDescFont, GearGraphics.WhiteBrush, 143f, 415f);
            g.DrawString(stat, GearGraphics.ClassSelectDescFont, GearGraphics.WhiteBrush, 143f, 447f);
            for (int i = pageIndex * 7; i < 42 + pageIndex * 7; i++)
            {
                if (pageIndex >= job_list.Count) continue;
                string jobID = job_list[i].ToString();
                var imgObj = Resource.ResourceManager.GetObject("ClassSelect_list_class_buttonclassEnabled_" + jobID + "_normal_0");
                if (imgObj is Bitmap bitmap) g.DrawImage(bitmap, 378 + 161 * (i / 7), 116 + 81 * (i % 7));
            }
            g.DrawImage(Resource.ClassSelect_list_class_layerclassCover, 375 + 161 * (selectIndex / 7), 113 + 81 * (selectIndex % 7));
            g.DrawString(job_list.Count.ToString(), GearGraphics.ClassSelectDescFont, GearGraphics.WhiteBrush, 1111f, 16f);
            foreach (AControl aCtrl in this.aControls)
            {
                aCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private IEnumerable<AControl> aControls
        {
            get
            {
                yield return hScroll;
                yield return btnYes;
                yield return btnNo;
                foreach (var btn in btnSelectedJobs)
                {
                    yield return btn;
                }
            }
        }

        private void btnSelectedJob_MouseClick(object sender, MouseEventArgs e)
        {
            ACtrlButton clickedButton = sender as ACtrlButton;
            if (clickedButton != null)
            {
                int buttonIndex = btnSelectedJobs.IndexOf(clickedButton);
                if (buttonIndex >= 0)
                {
                    selectIndex = buttonIndex + pageIndex * 7;
                    if (selectIndex >= job_list.Count)
                    {
                        selectIndex -= 7;
                    }
                    waitForRefresh = true;
                    Refresh();
                }
            }
        }

        private void btnYes_MouseClick(object sender, MouseEventArgs e)
        {
            this.jobIndex = selectIndex + pageIndex * 7;
            this.Visible = false;
        }

        private void btnNo_MouseClick(object sender, MouseEventArgs e)
        {
            this.Visible = false;
        }

        private void hScroll_ValueChanged(object sender, EventArgs e)
        {
            this.waitForRefresh = true;
        }

        private void aCtrl_RefreshCall(object sender, EventArgs e)
        {
            this.waitForRefresh = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            selectIndex += 1;
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseDown(childArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            selectIndex -= 1;
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseUp(childArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseClick(childArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseClick(e);
        }
    }
}
