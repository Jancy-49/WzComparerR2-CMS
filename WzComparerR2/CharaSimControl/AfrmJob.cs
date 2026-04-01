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
using DevComponents.AdvTree;
using System.Security.Policy;

namespace WzComparerR2.CharaSimControl
{
    public class AfrmJob : AlphaForm
    {
        public AfrmJob()
        {
            this.AllowDrop = true;
            initCtrl();
        }
        private Point baseOffset;
        private Point newLocation;
        private bool waitForRefresh;
        private Character character;

        private ACtrlHScroll hScroll;
        public ACtrlButton btnYes;
        private ACtrlButton btnNo;
        private ACtrlButton pagePrev;
        private ACtrlButton pageNext;
        private List<ACtrlButton> btnSelectedJobs = new List<ACtrlButton>();

        public int jobIndex = 0; //最终选择的职业索引
        private int selectIndex = 0; //当前选择的职业索引
        private int pageIndex = 0; //当前页码索引
        private int scrollValue = 0;
        public int selectJob { get; private set; } //最终选择的职业ID
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

            this.pagePrev = new ACtrlButton();
            this.pagePrev.Normal = new BitmapOrigin(Resource.ClassSelect_list_pagePrev_11);
            this.pagePrev.MouseOver = new BitmapOrigin(Resource.ClassSelect_list_pagePrev_11);
            this.pagePrev.Pressed = new BitmapOrigin(Resource.ClassSelect_list_pagePrev_11);
            this.pagePrev.Disabled = new BitmapOrigin(Resource.ClassSelect_list_pagePrev_11);
            this.pagePrev.Location = new Point(378, 685);
            this.pagePrev.Size = new Size(16, 15);
            this.pagePrev.Visible = false;
            this.pagePrev.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.pagePrev.MouseClick += new MouseEventHandler(pagePrev_MouseClick);

            this.pageNext = new ACtrlButton();
            this.pageNext.Normal = new BitmapOrigin(Resource.ClassSelect_list_pageNext_11);
            this.pageNext.MouseOver = new BitmapOrigin(Resource.ClassSelect_list_pageNext_11);
            this.pageNext.Pressed = new BitmapOrigin(Resource.ClassSelect_list_pageNext_11);
            this.pageNext.Disabled = new BitmapOrigin(Resource.ClassSelect_list_pageNext_11);
            this.pageNext.Location = new Point(1319, 685);
            this.pageNext.Size = new Size(16, 15);
            this.pageNext.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.pageNext.MouseClick += new MouseEventHandler(pageNext_MouseClick);

            btns();
        }

        private void btns()
        {
            for (int i = 0; i < 42; i++)
            {
                var btnSelectedJob = new ACtrlButton();
                btnSelectedJob.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/Login.img/ClassSelect/list/class/layer:classCover"), PluginBase.PluginManager.FindWz);
                btnSelectedJob.Location = new Point(375 + 161 * (i / 7), 113 + 81 * (i % 7));
                btnSelectedJob.Size = new Size(152, 74);
                btnSelectedJob.Visible = true;
                btnSelectedJob.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
                btnSelectedJob.MouseClick += new MouseEventHandler(btnSelectedJob_MouseClick);
                btnSelectedJobs.Add(btnSelectedJob);
            }
        }

        public override void Refresh()
        {
            this.preRender();
            this.SetBitmap(this.Bitmap);
            this.CaptionRectangle = new Rectangle(this.baseOffset, new Size(1366, 24));
            this.Location = newLocation;
            base.Refresh();
        }

        private void preRender()
        {
            if (Bitmap != null)
                Bitmap.Dispose();
            Size size = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/Login.img/ClassSelect/back/0/1/0"), PluginBase.PluginManager.FindWz).Bitmap.Size;

            //处理偏移
            this.newLocation = new Point(this.Location.X + this.baseOffset.X,
                this.Location.Y + this.baseOffset.Y);

            control_event();

            //绘制图像
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bitmap);
            renderBase(g);

            g.Dispose();
            this.Bitmap = bitmap;
        }

        private void control_event()
        {
            pagePrev.Visible = pageIndex != 0;
            pageNext.Visible = (pageIndex * 7 + 42) < job_list.Count;
            this.hScroll.Maximum = job_list.Count > 42 ? job_list.Count / 7 - 5 : 0;
            for (int i = 0; i < 42; i++)
            {
                btnSelectedJobs[i].Visible = (i + pageIndex * 7) < job_list.Count;
            }
            if (selectIndex < 0) selectIndex = 0;//防报错
        }

        private void renderBase(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            render_bitmap(g, "UI/Login.img/ClassSelect/back/0/1/0", 0, 0);
            Wz_Node charnode = PluginBase.PluginManager.FindWz($"UI/Login.img/ClassSelect/back/1/{job_list[selectIndex].ToString()}/0");
            Bitmap charBitmap = BitmapOrigin.CreateFromNode(charnode, PluginBase.PluginManager.FindWz).Bitmap;
            g.DrawImage(charBitmap, (1366 - charBitmap.Width) / 2, 0);
            render_bitmap(g, "UI/_Canvas/Login.img/ClassSelect/layer:aboveSpine", 0, 0);
            render_bitmap(g, "UI/Login.img/ClassSelect/list/backgrnd1", 378, 81);
            g.DrawImage(Resource.ClassSelect_back_2_0, 50, 13);
            render_bitmap(g, $"UI/Login.img/ClassSelect/desc/info/{job_list[selectIndex].ToString()}/className", 48, 44);
            render_bitmap(g, $"UI/Login.img/ClassSelect/desc/info/{job_list[selectIndex].ToString()}/jobMark", 50, 169);
            string subName = PluginManager.FindWz($@"UI/Login.img/ClassSelect/desc/info/{job_list[selectIndex].ToString()}/subName").GetValueEx<string>(null);
            string desc = PluginManager.FindWz($@"UI/Login.img/ClassSelect/desc/info/{job_list[selectIndex].ToString()}/desc").GetValueEx<string>(null).Replace("\\n", "\r\n");
            string race = PluginManager.FindWz($@"UI/Login.img/ClassSelect/desc/info/{job_list[selectIndex].ToString()}/race").GetValueEx<string>(null).Replace("\\n", "\r\n");
            string move = PluginManager.FindWz($@"UI/Login.img/ClassSelect/desc/info/{job_list[selectIndex].ToString()}/move").GetValueEx<string>(null).Replace("\\n", "\r\n");
            string stat = PluginManager.FindWz($@"UI/Login.img/ClassSelect/desc/info/{job_list[selectIndex].ToString()}/stat").GetValueEx<string>(null).Replace("\\n", "\r\n");
            g.DrawString(subName, GearGraphics.ClassSelectFontBold, GearGraphics.WhiteBrush, 48f, 193f);
            int picH = 233;
            GearGraphics.DrawPlainText(g, desc, GearGraphics.ClassSelectDescFont, Color.FromArgb(255, 255, 255), 50, 305, ref picH, 16);
            g.DrawString(race, GearGraphics.ClassSelectDescFont, GearGraphics.WhiteBrush, 143f, 356f);
            g.DrawString(move, GearGraphics.ClassSelectDescFont, GearGraphics.WhiteBrush, 143f, 388f);
            g.DrawString(stat, GearGraphics.ClassSelectDescFont, GearGraphics.WhiteBrush, 143f, 421f);
            for (int i = pageIndex * 7; i < 42 + pageIndex * 7; i++)
            {
                if ((i + 1) > job_list.Count) continue;
                render_bitmap(g, $"UI/Login.img/ClassSelect/list/class/button:classEnabled/{job_list[i].ToString()}/normal/0", 378 + 161 * (i / 7 - pageIndex), 116 + 81 * (i % 7));
            }
            render_bitmap(g, "UI/_Canvas/Login.img/ClassSelect/list/class/layer:classCover", 375 + 161 * (selectIndex / 7 - pageIndex), 113 + 81 * (selectIndex % 7));
            //g.DrawImage(Resource.ClassSelect_list_class_layerclassCover, 375 + 161 * (selectIndex / 7 - pageIndex), 113 + 81 * (selectIndex % 7));
            g.DrawString(job_list.Count.ToString(), GearGraphics.ClassSelectDescFont, GearGraphics.WhiteBrush, 1111f, 16f);

            foreach (AControl aCtrl in this.aControls)
            {
                aCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void render_bitmap(Graphics g, string nodepath, int x, int y)
        {
            Wz_Node Node = PluginBase.PluginManager.FindWz(nodepath);
            Bitmap image = BitmapOrigin.CreateFromNode(Node, PluginBase.PluginManager.FindWz).Bitmap;
            g.DrawImage(image, x, y);
        }

        private IEnumerable<AControl> aControls
        {
            get
            {
                yield return hScroll;
                yield return btnYes;
                yield return btnNo;
                yield return pagePrev;
                yield return pageNext;
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
                    waitForRefresh = true;
                    Refresh();
                }
            }
        }

        private void btnYes_MouseClick(object sender, MouseEventArgs e)
        {
            this.jobIndex = selectIndex;
            this.selectJob = job_list[jobIndex];
            this.Visible = false;
            if (this.Owner is MainForm mainForm)
            {
                mainForm.buttonJobSelect.Checked = false;
            }
        }

        private void btnNo_MouseClick(object sender, MouseEventArgs e)
        {
            this.Visible = false;
        }

        private void pagePrev_MouseClick(object sender, MouseEventArgs e)
        {
            pageIndex -= 1;
            this.hScroll.Value = pageIndex;
            this.scrollValue = this.hScroll.Value;
            if (selectIndex >= ((pageIndex + 5) * 7 - 1) && selectIndex < (pageIndex + 6) * 7)
                selectIndex -= 7;
            waitForRefresh = true;
            Refresh();
        }

        private void pageNext_MouseClick(object sender, MouseEventArgs e)
        {
            pageIndex += 1;
            this.hScroll.Value = pageIndex;
            this.scrollValue = this.hScroll.Value;
            if (selectIndex >= 0 && selectIndex < (pageIndex + 1) * 7)
                selectIndex += 7;
            waitForRefresh = true;
            Refresh();
        }

        private void hScroll_ValueChanged(object sender, EventArgs e)
        {
            this.scrollValue = this.hScroll.Value;
            pageIndex = scrollValue;
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

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseWheel(childArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseWheel(e);
        }
    }
}
